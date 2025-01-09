using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Domain.Enums;
using System.Data.Common;
using System.Linq.Expressions;
using Contract = FranchiseProject.Domain.Entity.Contract;
using Microsoft.AspNetCore.Mvc;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using Org.BouncyCastle.Pqc.Crypto.Falcon;
using FranchiseProject.Application.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.WebSockets;
using FranchiseProject.Application.ViewModels.PackageViewModels;

namespace FranchiseProject.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly IClaimsService _clamsService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateContractViewModel> _validator;
        private readonly IValidator<UpdateContractViewModel> _validatorUpdate;
        private readonly IValidator<CreateContractLicenseModel> _createContractLicenseValidator;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public ContractService(IConfiguration configuration,IValidator<UpdateContractViewModel> validatorUpdate, IEmailService emailService,
            IPdfService pdfService, IMapper mapper, IUnitOfWork unitOfWork, 
            IValidator<CreateContractViewModel> validator, IClaimsService claimsService,
            IValidator<CreateContractLicenseModel> createContractLicenseValidator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _clamsService = claimsService;
            _pdfService = pdfService;
            _emailService = emailService;
            _validatorUpdate = validatorUpdate;
            _configuration = configuration;
            _createContractLicenseValidator = createContractLicenseValidator;
        }
        public async Task NotifyCustomersOfExpiringContracts()
        {
            var agencies = await _unitOfWork.AgencyRepository.GetAgencyExpiredAsync();

            foreach (var agency in agencies)
            {
                if (agency == null || string.IsNullOrEmpty(agency.Email))
                {

                    continue;
                }

                var emailMessage = new MessageModel
                {
                    To = agency.Email,
                    Subject = "[futuretech-noreply] Thông Báo Hết Hạn Hợp Đồng",
                    Body = $"<p>Kính gửi {agency.Name},</p>" +
                          $"<p>Chúng tôi xin thông báo rằng hợp đồng của bạn với Futuretech đã đến hạn kết thúc.</p>" +
                          $"<p>Để đảm bảo việc hợp tác được duy trì và các dịch vụ không bị gián đoạn, chúng tôi kính mời bạn liên hệ với đội ngũ của chúng tôi để thực hiện các thủ tục gia hạn hợp đồng.</p>" +
                          $"<p>Vui lòng sử dụng các thông tin dưới đây để liên hệ:</p>" +
                          $"<ul>" +
                          $"<li><strong>Email hỗ trợ:</strong> support@futuretech.com</li>" +
                          $"<li><strong>Số điện thoại:</strong> 0123-456-789</li>" +
                          $"</ul>" +
                          $"<p>Nếu bạn đã hoàn thành gia hạn hợp đồng, vui lòng bỏ qua email này.</p>" +
                          $"<p>Chúng tôi mong muốn tiếp tục đồng hành cùng bạn trên con đường phát triển sắp tới.</p>" +
                          $"<p>Trân trọng,</p>" +
                          $"<p>Đội ngũ Futuretech</p>"
                };

                await _emailService.SendEmailAsync(emailMessage);

            }
        }
        public async Task<ApiResponse<bool>> UploadContractAsync(CreateContractViewModel create)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var validationResult = await _validator.ValidateAsync(create);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var agencyId = Guid.Parse(create.AgencyId);
                var existAgency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (existAgency == null) return ResponseHandler.Success(false,"Không tìm thấy đối tác");

                if (existAgency.Status != AgencyStatusEnum.Processing) return ResponseHandler.Success(false, "Đối tác chưa thể đăng kí nhượng quyền.");

                var activeContract = await _unitOfWork.ContractRepository.GetActiveContractByAgencyIdAsync(agencyId);

                if (activeContract != null) return ResponseHandler.Success(false, "Đối tác đã có hợp đồng đang trong thời hạn.");

                var package = await _unitOfWork.PackageRepository.GetExistByIdAsync((Guid)create.PackageId);
                if(package == null) return ResponseHandler.Success(false, "Không tìm thấy gối bản quyền phù hợp!");

                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                contract.ContractCode = await GenerateUniqueContractCode();
                var franchiseFee = await _unitOfWork.FranchiseFeesRepository.GetAllAsync();
                contract.Title = create.Title;
                contract.StartTime = create.StartTime;
                contract.EndTime = create.EndTime;
                contract.ContractDocumentImageURL = create.ContractDocumentImageURL;
                contract.RevenueSharePercentage = create.RevenueSharePercentage;
                contract.DepositPercentage = create.DepositPercentage;
                contract.Status = ContractStatusEnum.None;
                contract.ContractCode = await GenerateUniqueContractCode();
                contract.FrachiseFee = franchiseFee.Sum(f => f.FeeAmount);
                contract.Total = contract.FrachiseFee + package.Price;

                 _unitOfWork.ContractRepository.Update(contract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(existAgency.Email, contract.ContractDocumentImageURL);
                    if (!emailResponse.isSuccess) return ResponseHandler.Success(true, "Tải lên thành công, nhưng không thể gửi email đính kèm hợp đồng.");


                    response = ResponseHandler.Success(true, "Tải lên thành công !");
                }
                else
                {
                    throw new Exception("Create unsuccesfully ");
                }
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UploadContractLicenseAsync(CreateContractLicenseModel create) 
        {
            var response = new ApiResponse<bool>();
            try
            {
                var validationResult = await _createContractLicenseValidator.ValidateAsync(create);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var agencyId = create.AgencyId;
                var existAgency = await _unitOfWork.AgencyRepository.GetByIdAsync((Guid)agencyId);
                if (existAgency == null) return ResponseHandler.Success(false, "Không tìm thấy đối tác");

                if (existAgency.Status != AgencyStatusEnum.Processing) return ResponseHandler.Success(false, "Đối tác chưa thể đăng kí nhượng quyền.");

                var activeContract = await _unitOfWork.ContractRepository.GetActiveContractByAgencyIdAsync((Guid)agencyId);

                if (activeContract != null) return ResponseHandler.Success(false, "Đối tác đã có hợp đồng đang trong thời hạn.");

                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync((Guid)agencyId);
                contract.ContractCode = await GenerateUniqueContractCode();
                var franchiseFee = await _unitOfWork.FranchiseFeesRepository.GetAllAsync();
                contract.Title = create.Title;
                contract.StartTime = create.StartTime;
                contract.EndTime = create.EndTime;
                contract.ContractDocumentImageURL = create.ContractDocumentImageURL;
                contract.RevenueSharePercentage = create.RevenueSharePercentage;
                contract.DepositPercentage = create.DepositPercentage;
                contract.Status = ContractStatusEnum.None;
                contract.ContractCode = await GenerateUniqueContractCode();

                contract.FrachiseFee = franchiseFee.Sum(f => f.FeeAmount);
                var packageFee= create.CreatePackageModel.Price;
                contract.Total=contract.FrachiseFee+packageFee;// Tong gia tri hop dong

                var package = _mapper.Map<Package>(create.CreatePackageModel);
                await _unitOfWork.PackageRepository.AddAsync(package);

                contract.PackageId = package.Id; 
                _unitOfWork.ContractRepository.Update(contract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(existAgency.Email, contract.ContractDocumentImageURL);
                    if (!emailResponse.isSuccess) return ResponseHandler.Success(true, "Tải lên thành công, nhưng không thể gửi email đính kèm hợp đồng.");
                    

                    response = ResponseHandler.Success(true , "Tải lên thành công !");
                }
                else
                {
                    throw new Exception("Create unsuccesfully ");
                }
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> AgencyUploadContractAsync(string ContractDocumentImageURL,Guid AgencyId)
        {
            var response = new ApiResponse<bool>();
            try
            {
               
             //   var agencyId = Guid.Parse(create.AgencyId);
                var existAgency = await _unitOfWork.AgencyRepository.GetByIdAsync(AgencyId);
                if (existAgency == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy đối tác ";
                    return response;
                }
                if (existAgency.Status != AgencyStatusEnum.Processing)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Đối tác chưa thể đăng kí nhượng quyền.";
                    return response;
                }
                var activeContract = await _unitOfWork.ContractRepository.GetActiveContractByAgencyIdAsync(AgencyId);

                if (activeContract != null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Đối tác đã có hợp đồng đang trong thời hạn.";
                    return response;
                }
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(AgencyId);
               // contract.ContractCode = await GenerateUniqueContractCode();
                var franchiseFee = await _unitOfWork.FranchiseFeesRepository.GetAllAsync();
            
                contract.ContractDocumentImageURL = ContractDocumentImageURL;

                // contract.FrachiseFee = franchiseFee.Sum(f => f.FeeAmount);
                _unitOfWork.ContractRepository.Update(contract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(existAgency.Email, contract.ContractDocumentImageURL);
                    if (!emailResponse.isSuccess)
                    {
                        response.Message = "Tải lên thành công, nhưng không thể gửi email đính kèm hợp đồng.";
                    }
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tải lên thành công !";
                }
                else
                {
                    throw new Exception("Create unsuccesfully ");
                }
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateContractAsync(UpdateContractViewModel update, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var contractId = Guid.Parse(id);
                FluentValidation.Results.ValidationResult validationResult = await _validatorUpdate.ValidateAsync(update);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var existingContract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (existingContract == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = " không tìm thấy hợp đồng";
                    return response;
                }
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(existingContract.AgencyId.Value);
                existingContract.Title = update.Title;
                existingContract.ContractDocumentImageURL = update.ContractDocumentImageURL;
                //   existingContract.AgencyId = Guid.Parse(update.AgencyId);
                existingContract.StartTime = update.StartTime;
                existingContract.EndTime = update.EndTime;
                existingContract.RevenueSharePercentage = update.RevenueSharePercentage;
                _unitOfWork.ContractRepository.Update(existingContract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();

                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(agency?.Email, existingContract.ContractDocumentImageURL);
                    if (!emailResponse.isSuccess)
                    {
                        response.Message = "Cập nhật thành Công, nhưng không thể gửi email đính kèm hợp đồng.";
                    }
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Cập nhật thành Công !";
                }
                else
                {
                    throw new Exception("Update unsuccesfully ");
                }


            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractViewModelAsync(FilterContractViewModel filterContractModel)
        {
            var response = new ApiResponse<Pagination<ContractViewModel>>();

            try
            {
                Expression<Func<Contract, bool>> filter = c =>
                 c.Status != ContractStatusEnum.None &&
                   (!filterContractModel.StartTime.HasValue || filterContractModel.StartTime <= c.StartTime) &&
            (!filterContractModel.EndTime.HasValue || filterContractModel.EndTime >= c.EndTime) &&
            (!filterContractModel.Status.HasValue || c.Status == filterContractModel.Status) &&
            (!filterContractModel.AgencyId.HasValue || c.AgencyId == filterContractModel.AgencyId) &&
            (string.IsNullOrEmpty(filterContractModel.SearchInput) ||
                (c.Title.Contains(filterContractModel.SearchInput) ||
                (c.Agency != null && c.Agency.Name.Contains(filterContractModel.SearchInput)))
            );

                var contracts = await _unitOfWork.ContractRepository.GetFilterAsync(
                    filter: filter,
                    orderBy: q => q.OrderByDescending(c => c.CreationDate),
                    pageIndex: filterContractModel.PageIndex,
                    pageSize: filterContractModel.PageSize,
                    includeProperties: "Agency"
                );

                var contractViewModels = new Pagination<ContractViewModel>
                {
                    Items = contracts.Items.Select(c => new ContractViewModel
                    {
                        Id = c.Id,
                        Title = c.Title,
                        ContractCode = c.ContractCode,
                        StartTime = c.StartTime ?? DateTime.MinValue,
                        EndTime = c.EndTime ?? DateTime.MinValue,
                        ContractDocumentImageURL = c.ContractDocumentImageURL,
                        Total = c.Total,
                        DepositPercentage = c.DepositPercentage,
                        PaidAmount = c.PaidAmount,
                        FrachiseFee = c.FrachiseFee,
                        Status = c.Status,
                        RevenueSharePercentage = c.RevenueSharePercentage,
                        AgencyName = c.Agency != null ? c.Agency.Name : string.Empty,
                        UsedAccountCount = c.UsedAccountCount,
                        PackageViewModel = c.Package != null ? new PackageViewModel
                        {
                            Id = c.Package.Id,
                            Name = c.Package.Name,
                            Description = c.Package.Description,
                            Price = c.Package.Price,
                            NumberOfUsers = c.Package.NumberOfUsers,
                            Status = c.Package.Status
                        } : null
                    }).ToList(),
                    TotalItemsCount = contracts.TotalItemsCount,
                    PageIndex = contracts.PageIndex,
                    PageSize = contracts.PageSize
                };

                if (!contractViewModels.Items.Any())
                {
                    return ResponseHandler.Success(contractViewModels, "Không tìm thấy hợp đồng phù hợp!");
                }

                response = ResponseHandler.Success(contractViewModels, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<ContractViewModel>>(ex.Message);
            }

            return response;
        }
        private List<ContractViewModel> MapContractsToViewModels(List<Contract> contracts)
        {
            var contractViewModels = new List<ContractViewModel>();

            foreach (var contract in contracts)
            {
                var contractViewModel = new ContractViewModel
                {
                    Id = contract.Id,
                    Title = contract.Title,
                    StartTime = contract.StartTime ?? DateTime.MinValue,
                    
                    EndTime = contract.EndTime ?? DateTime.MinValue,
                    ContractCode=contract.ContractCode,
                    ContractDocumentImageURL = contract.ContractDocumentImageURL,
                    RevenueSharePercentage = contract.RevenueSharePercentage,
                    AgencyName = contract.Agency != null ? contract.Agency.Name : string.Empty,
                    UsedAccountCount=contract.UsedAccountCount

                };

                contractViewModels.Add(contractViewModel);
            }

            return contractViewModels;
        }
        public async Task<ApiResponse<ContractViewModel>> GetContractByIdAsync(string id)
        {
            var response = new ApiResponse<ContractViewModel>();
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetByIdAsync(Guid.Parse(id));
                if (contract == null)
                {
                    return ResponseHandler.Success<ContractViewModel>(null, "Không tìm thấy hợp đồng");
                }

                var contractViewModel = _mapper.Map<ContractViewModel>(contract);
                contractViewModel.AgencyName = contract.Agency?.Name;

                if (contract.Package != null)
                {
                    contractViewModel.PackageViewModel = new PackageViewModel
                    {
                        Id = contract.Package.Id,
                        Name = contract.Package.Name,
                        Description = contract.Package.Description,
                        Price = contract.Package.Price,
                        NumberOfUsers = contract.Package.NumberOfUsers,
                        Status = contract.Package.Status
                    };
                }

                return ResponseHandler.Success(contractViewModel, "Truy xuất thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<ContractViewModel>($"Lỗi khi truy xuất hợp đồng: {ex.Message}");
            }
        }
        public async Task<ApiResponse<ContractViewModel>> GetContractbyAgencyId(Guid agencyId)
        {
            var response = new ApiResponse<ContractViewModel>();
            try
            {
                var contract =await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                if (contract == null)
                {
                    return ResponseHandler.Success<ContractViewModel>(null, "Không tìm thấy hợp đồng");
                }
                var contractViewModel = _mapper.Map<ContractViewModel>(contract);
                var agency =await _unitOfWork.AgencyRepository.GetByIdAsync(contract.AgencyId.Value);
                contractViewModel.AgencyName = agency.Name;
                if (contract.Package != null)
                {
                    contractViewModel.PackageViewModel = new PackageViewModel
                    {
                        Id = contract.Package.Id,
                        Name = contract.Package.Name,
                        Description = contract.Package.Description,
                        Price = contract.Package.Price,
                        NumberOfUsers = contract.Package.NumberOfUsers,
                        Status = contract.Package.Status
                    };
                }
                return ResponseHandler.Success(contractViewModel, "Truy xuất hợp đồng thành công");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<ContractViewModel>($"Lỗi truy xuất hợp đồng: {ex.Message}");
            }
        }
        public async Task<ApiResponse<string>> UploadContractTemplateAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return ResponseHandler.Failure<string>("Không có file được chọn.");
                }

                if (!file.FileName.EndsWith(".doc") && !file.FileName.EndsWith(".docx"))
                {
                    return ResponseHandler.Failure<string>("File phải có định dạng .doc hoặc .docx");
                }

                using (var stream = file.OpenReadStream())
                {
                    string fileName = $"ContractTemplate_{DateTime.Now:yyyyMMddHHmmss}.docx";
                    string firebaseUrl = await _unitOfWork.FirebaseRepository.UploadFileAsync(stream, fileName);
                    var configuration = (IConfigurationRoot)_configuration;
                    configuration["ContractTemplateUrl"] = firebaseUrl;


                    return ResponseHandler.Success(firebaseUrl, "Template hợp đồng đã được tải lên thành công.");
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"Lỗi khi tải lên template hợp đồng: {ex.Message}");
            }
        }
        public async Task<ApiResponse<string>> DownloadContractAsPdfAsync(Guid agencyId)
        {
            try
            {
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);

                if (contract == null)
                {
                    return ResponseHandler.Success<string>(null, "Không tìm thấy hợp đồng cho đối tác này.");
                }

                if (agency == null)
                {
                    return ResponseHandler.Success<string>(null, "Không tìm thấy thông tin .");
                }

                int totalMonths = (int)((contract.EndTime?.Year - contract.StartTime?.Year) * 12
                    + (contract.EndTime?.Month - contract.StartTime?.Month));
              if(contract.DepositPercentage==null&& contract.RevenueSharePercentage==null)
                {
                    return ResponseHandler.Success<string>(null, "Không tìm thấy thông tin .");
                }
                var address = $"{agency.Address}, {agency.Ward}, {agency.District}, {agency.City}";
                var deposit = contract.Total * (contract.DepositPercentage / 100);

                var inputInfo = new InputContractViewModel
                {
                    FranchiseFee = contract.FrachiseFee,
                    TotalMoney = contract.Total,
                    Deposit = deposit,
                    ContractCode = contract.ContractCode,
                    Druration = totalMonths,
                    Percent = contract.RevenueSharePercentage,
                    Address = address
                };

                using (var pdfStream = await _pdfService.FillDocumentTemplate(inputInfo))
                {
                    if (pdfStream == null)
                    {
                        return ResponseHandler.Success<string>(null, "Không thể tạo file PDF từ template.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await pdfStream.CopyToAsync(memoryStream);
                        byte[] pdfBytes = memoryStream.ToArray();

                        if (string.IsNullOrEmpty(contract.ContractCode))
                        {
                            contract.ContractCode = inputInfo.ContractCode;
                            _unitOfWork.ContractRepository.Update(contract);
                            await _unitOfWork.SaveChangeAsync();
                        }

                        string fileName = $"Contract_{contract.ContractCode}.doc";
                        using (var uploadStream = new MemoryStream(pdfBytes))
                        {
                            string firebaseUrl = await _unitOfWork.FirebaseRepository.UploadFileAsync(uploadStream, fileName);

                            if (string.IsNullOrEmpty(firebaseUrl))
                            {
                                return ResponseHandler.Success<string>(null, "Không thể tải file lên Firebase.");
                            }

                            return ResponseHandler.Success(firebaseUrl, "File hợp đồng đã được tải lên thành công.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ResponseHandler.Success<string>(null, $"Lỗi khi tạo và tải lên file hợp đồng: {ex.Message}");
            }
        }
        private async Task<string> GenerateUniqueContractCode()
        {
            string contractCode;
            bool isUnique;
            do
            {
                // Tạo mã dựa trên timestamp và chuỗi ngẫu nhiên
                var timestamp = DateTime.Now.ToString("yyMMddHHmm");
                var random = new Random();
                var randomPart = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                contractCode = timestamp + randomPart;

                // Kiểm tra xem mã đã tồn tại chưa
                isUnique = !await _unitOfWork.ContractRepository.AnyAsync(c => c.ContractCode == contractCode);
            } while (!isUnique);

            return contractCode;
        }
    }
}

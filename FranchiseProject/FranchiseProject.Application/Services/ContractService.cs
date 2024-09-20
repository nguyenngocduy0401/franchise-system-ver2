using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Application.ViewModels.ContractViewModel;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FranchiseProject.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly IClaimsService _clamsService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateContractViewModel> _validator;
        private readonly IValidator<UpdateContractViewModel> _validatorUpdate;
        private readonly IPdfService _pdfService;
        private readonly IFirebaseService _firebaseService;
        private readonly IEmailService _emailService;

        public ContractService(IValidator<UpdateContractViewModel> validatorUpdate,IEmailService emailService,IPdfService pdfService,IFirebaseService firebaseService,IMapper mapper,IUnitOfWork unitOfWork,IValidator<CreateContractViewModel> validator,IClaimsService claimsService)
        {
            _mapper = mapper;
                _unitOfWork = unitOfWork;
                _validator = validator;
            _clamsService = claimsService;
            _pdfService = pdfService;
              _firebaseService = firebaseService;
            _emailService = emailService;
            _validatorUpdate = validatorUpdate;
        }


            public async Task<ApiResponse<bool>> CreateContractAsync(CreateContractViewModel create)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(create);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var agencyId= Guid.Parse(create.AgencyId);
                var existAgency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if(existAgency == null)
                {
                    response.Data=false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy đối tác ";
                    return response;
                }
                if(existAgency.Status== AgencyStatusEnum.Pending || existAgency.Status == AgencyStatusEnum.Processing )
                {
                    response.Data = false;
                    response.isSuccess=true;
                    response.Message = "Đối tác chưa thể đăng kí nhượng quyền.";
                    return response;
                }
                var activeContract = await _unitOfWork.ContractRepository.GetActiveContractByAgencyIdAsync(agencyId);

                if (activeContract != null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Đối tác đã có hợp đồng đang trong thời hạn.";
                    return response;
                }
                var contract = _mapper.Map<FranchiseProject.Domain.Entity.Contract>(create);
                contract.StartTime = DateTime.Now;
                contract.EndTime = contract.StartTime.AddYears(contract.Duration);
                //xu li pdf
                var pdfStream = _pdfService.FillPdfTemplate(create);
                var fileName = $"Contract_{Guid.NewGuid()}.pdf";
                var contractDocumentUrl = await _firebaseService.UploadFileAsync(pdfStream, fileName);
                contract.ContractDocumentImageURL = contractDocumentUrl;
                //
                await _unitOfWork.ContractRepository.AddAsync(contract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(existAgency.Email, contractDocumentUrl);
                    if (!emailResponse.isSuccess)
                    {
                        response.Message = "Tạo Thành Công, nhưng không thể gửi email đính kèm hợp đồng.";
                    }
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
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
               var existingContract= await _unitOfWork.ContractRepository.GetByIdAsync(contractId);
                if (existingContract == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = " không tìm thấy hợp đồng";
                    return response;
                }
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(existingContract.AgencyId.Value);
                existingContract.Amount = update.Amount;
                existingContract.Duration= update.Duration;
                existingContract.Description=update.Description;
                existingContract.TermsAndCondition=update.TermsAndCondition;
                existingContract.StartTime = DateTime.Now;
                existingContract.EndTime = existingContract.StartTime.AddYears(update.Duration);
                //xu li pdf
                var pdfStream = _pdfService.FillUpdatePdfTemplate(update);
                var fileName = $"Contract_{Guid.NewGuid()}.pdf";
                var contractDocumentUrl = await _firebaseService.UploadFileAsync(pdfStream, fileName);
                existingContract.ContractDocumentImageURL = contractDocumentUrl;
                //
                 _unitOfWork.ContractRepository.Update(existingContract);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
               
                if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendContractEmailAsync(agency?.Email, contractDocumentUrl);
                    if (!emailResponse.isSuccess)
                    {
                        response.Message = "Tạo Thành Công, nhưng không thể gửi email đính kèm hợp đồng.";
                    }
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
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
    

      

        public async Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractViewModelAsync(FilterContractViewModel filter)
        {
            var response = new ApiResponse<Pagination<ContractViewModel>>();

            try
            {
                DateTime? start = null;
                DateTime? end = null;


                if (!string.IsNullOrEmpty(filter.StartTime))
                {
                    start = DateTime.Parse(filter.StartTime);
                }

                if (!string.IsNullOrEmpty(filter.EndTime))
                {
                    end = DateTime.Parse(filter.EndTime);
                }
                var paginationResult = await _unitOfWork.ContractRepository.GetFilterAsync(
                      filter: s =>
                       (!start.HasValue || s.StartTime >= start.Value) &&
                (!end.HasValue || s.EndTime <= end.Value) &&
                (!start.HasValue || !end.HasValue ||
                 (s.StartTime >= start.Value && s.EndTime <= end.Value)),
                    pageIndex: filter.PageIndex,
                    pageSize: filter.PageSize
                );
                var contractViewModels = new List<ContractViewModel>();
                foreach (var contract in paginationResult.Items)
                {
                    if(contract.AgencyId.HasValue) 
    {
                        var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(contract.AgencyId.Value); 
                        var contractViewModel = _mapper.Map<ContractViewModel>(contract);
                        contractViewModel.AgencyName = agency?.Name;
                        contractViewModels.Add(contractViewModel);
                    }
                    else
                    {
                        var contractViewModel = _mapper.Map<ContractViewModel>(contract);
                        contractViewModel.AgencyName = "Unknown"; 
                        contractViewModels.Add(contractViewModel);
                    }
                }
                var paginationViewModel = new Pagination<ContractViewModel>
                {
                    PageIndex = paginationResult.PageIndex,
                    PageSize = paginationResult.PageSize,
                    TotalItemsCount = paginationResult.TotalItemsCount,
                    Items = contractViewModels
                };
                response.Data = paginationViewModel;
                response.isSuccess = true;
                response.Message = "Truy Xuất Thành Công ";
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

        public async Task<ApiResponse<ContractViewModel>> GetContractByIdAsync(string id)
        {
        var response = new ApiResponse<ContractViewModel>();
        try
        {
            var contract = await _unitOfWork.ContractRepository.GetByIdAsync(Guid.Parse(id));
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(contract.AgencyId.Value);
            if (contract == null)
            {
                response.Data = null;
                response.isSuccess = true;
                response.Message = "Không tìm thấy hợp đồng";
                return response;

            }

            var contractViewModel = _mapper.Map<ContractViewModel>(contract);
               contractViewModel.AgencyName = agency?.Name;
            response.Data = contractViewModel;
            response.isSuccess = true;
            response.Message = "Truy xuất thành công ";
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

     
    }
}

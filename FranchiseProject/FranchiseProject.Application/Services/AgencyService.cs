using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using System.Data.Common;
using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using Microsoft.IdentityModel.Tokens;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using Microsoft.AspNetCore.Identity;

namespace FranchiseProject.Application.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;
        private readonly IValidator<CreateAgencyViewModel> _validator;
        private readonly IValidator<UpdateAgencyViewModel> _validatorUpdate;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public AgencyService(IMapper mapper, IUnitOfWork unitOfWork,
            IClaimsService claimsService, IValidator<CreateAgencyViewModel> validator,
            IUserService userService, IHubContext<NotificationHub> hubContext,
            IEmailService emailService, IValidator<UpdateAgencyViewModel> validatorUpdate,
            ICurrentTime currentTime, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _claimsService = claimsService;
            _mapper = mapper;
            _userService = userService;
            _hubContext = hubContext;
            _emailService = emailService;
            _validatorUpdate = validatorUpdate;
            _currentTime = currentTime;
            _userManager = userManager;
        }

        public async Task<ApiResponse<bool>> CreateAgencyAsync(CreateAgencyViewModel create)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(create);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var agency = _mapper.Map<Agency>(create);
                agency.Status = AgencyStatusEnum.Processing;

              
                await _unitOfWork.AgencyRepository.AddAsync(agency);
                var consult = await _unitOfWork.FranchiseRegistrationRequestRepository.GetExistByIdAsync(create.RegisterFormId.Value);
                if (consult == null)
                {
                    return ResponseHandler.Success<bool>(false, "Không tìm thấy tư vấn !");
                }
                consult.Status = ConsultationStatusEnum.ProspectivePartner;

                var existingUser = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agency.Id);
                if (existingUser == null)
                {
                    var userCredentials = await _userService.GenerateUserCredentials(agency.Name);

                    var newUser = new User
                    {
                        UserName = userCredentials.UserName,
                        FullName = create.Name,
                        PasswordHash = userCredentials.Password,
                        AgencyId = agency.Id,
                        Status = UserStatusEnum.inactive,
                        Email = agency.Email,
                        PhoneNumber = agency.PhoneNumber,

                    };
                    var user2 = _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(newUser, RolesEnum.AgencyManager.ToString());
                    var emailMessage = new MessageModel
                    {
                        To = agency.Email,
                        Subject = "[futuretech-noreply] Thông Báo Cấp Tài Khoản",
                        Body = $"<p>Chào {agency.Name},</p>" +
                           $"<p>Chúng tôi rất vui mừng thông báo rằng bạn đã được cấp tài khoản để truy cập hệ thống của chúng tôi.</p>" +
                           $"<p>Thông tin tài khoản của bạn như sau:</p>" +
                           $"<ul>" +
                           $"<li><strong>Username:</strong> {newUser.UserName}</li>" +
                           $"<li><strong>Password:</strong> {userCredentials.Password}</li>" +
                           $"</ul>" +
                           $"<p>Vui lòng bảo mật thông tin đăng nhập này và không chia sẻ với bất kỳ ai.</p>" +
                           $"<p>Đây là bước đầu tiên để bạn có thể tiếp cận và khám phá các tính năng của hệ thống.</p>" +
                           $"<p>Nếu bạn cần hỗ trợ hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>" +
                           $"<p>Trân trọng,</p>" +
                           $"<p>Đội ngũ Futuretech</p>"
                    };
                    bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                    if (!emailSent)
                    {
                        response.Message += " (Lỗi khi gửi email)";
                    }
                    await user2;
                }
                _unitOfWork.FranchiseRegistrationRequestRepository.Update(consult);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
                    /*var emailMessage = EmailTemplate.AgencyRegistrationSuccess(agency.Email, agency.Name);
                    var emailSent = await _emailService.SendEmailAsync(emailMessage);
                    if (!emailSent)
                    {
                        response.Message += " (Lỗi khi gửi email)";
                    }*/
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
        public async Task<ApiResponse<Pagination<AgencyViewModel>>> FilterAgencyAsync(FilterAgencyViewModel filter)
        {
            var response = new ApiResponse<Pagination<AgencyViewModel>>();

            try
            {
                var paginationResult = await _unitOfWork.AgencyRepository.GetFilterAsync(
                      filter: s =>
                    (!filter.Status.HasValue || s.Status == filter.Status) &&
                    (string.IsNullOrEmpty(filter.SearchInput) || (
                    s.Name != null && s.Name.Contains(filter.SearchInput) ||
                    s.Address != null && s.Address.Contains(filter.SearchInput) ||
                    s.City != null && s.City.Contains(filter.SearchInput) ||
                    s.District != null && s.District.Contains(filter.SearchInput) ||
                    s.Ward != null && s.Ward.Contains(filter.SearchInput) ||
                    s.PhoneNumber != null && s.PhoneNumber.Contains(filter.SearchInput) ||
                    s.Email != null && s.Email.Contains(filter.SearchInput)
                )),
                    pageIndex: filter.PageIndex,
                    pageSize: filter.PageSize
                );
                var agencyViewModel = _mapper.Map<List<AgencyViewModel>>(paginationResult.Items);
                var paginationViewModel = new Pagination<AgencyViewModel>
                {
                    PageIndex = paginationResult.PageIndex,
                    PageSize = paginationResult.PageSize,
                    TotalItemsCount = paginationResult.TotalItemsCount,
                    Items = agencyViewModel
                };
                response.Data = paginationViewModel;
                response.isSuccess = true;
                response.Message = "Truy xuất thành công";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = "Error occurred while filtering suppliers: " + ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> UpdateAgencyAsync(UpdateAgencyViewModel update, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var agenyId = Guid.Parse(id);
                FluentValidation.Results.ValidationResult validationResult = await _validatorUpdate.ValidateAsync(update);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var existingAgency = await _unitOfWork.AgencyRepository.GetByIdAsync(agenyId);
                if (existingAgency == null)
                {
                    response.isSuccess = true;
                    response.Data = false;
                    response.Message = "Không tìm thấy đối tác ";
                    return response;
                }
                _mapper.Map(update, existingAgency);


                _unitOfWork.AgencyRepository.Update(existingAgency);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Cập nhật thành công!";
                }
                else
                {
                    throw new Exception("Update unsuccesfully");
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
        public async Task<ApiResponse<AgencyViewModel>> GetAgencyById(string id)
        {
            var response = new ApiResponse<AgencyViewModel>();
            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(Guid.Parse(id));
                if (agency == null)
                {
                    response.isSuccess = false;
                    response.Message = "Không tìm thấy đối tác";
                    return response;

                }

                var agencyViewModel = _mapper.Map<AgencyViewModel>(agency);
                response.Data = agencyViewModel;
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
        public async Task<ApiResponse<bool>> UpdateAgencyStatusAsync(string id, AgencyStatusEnum newStatus)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var agencyId = Guid.Parse(id);
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                if (agency == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy đối tác";
                    return response;
                }
                if (agency.Status == newStatus)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = $"Trạng thái đã là {newStatus}, không thể cập nhật lại.";
                    return response;
                }
                switch (newStatus)
                {
                    case AgencyStatusEnum.Processing:
                        var existingUser = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agencyId);
                        if (existingUser == null)
                        {
                            var userCredentials = await _userService.GenerateUserCredentials(agency.Name);

                            var newUser = new User
                            {
                                UserName = userCredentials.UserName,
                                PasswordHash = userCredentials.Password,
                                AgencyId = agencyId,
                                Status = UserStatusEnum.inactive,
                                Email = agency.Email,
                                PhoneNumber = agency.PhoneNumber,

                            };
                            var user2 = _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(newUser, RolesEnum.AgencyManager.ToString());
                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "[futuretech-noreply] Thông Báo Cấp Tài Khoản",
                                Body = $"<p>Chào {agency.Name},</p>" +
                                   $"<p>Chúng tôi rất vui mừng thông báo rằng bạn đã được cấp tài khoản để truy cập hệ thống của chúng tôi.</p>" +
                                   $"<p>Thông tin tài khoản của bạn như sau:</p>" +
                                   $"<ul>" +
                                   $"<li><strong>Username:</strong> {newUser.UserName}</li>" +
                                   $"<li><strong>Password:</strong> {userCredentials.Password}</li>" +
                                   $"</ul>" +
                                   $"<p>Vui lòng bảo mật thông tin đăng nhập này và không chia sẻ với bất kỳ ai.</p>" +
                                   $"<p>Đây là bước đầu tiên để bạn có thể tiếp cận và khám phá các tính năng của hệ thống.</p>" +
                                   $"<p>Nếu bạn cần hỗ trợ hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với đội ngũ hỗ trợ của chúng tôi.</p>" +
                                   $"<p>Trân trọng,</p>" +
                                   $"<p>Đội ngũ Futuretech</p>"
                            };
                            bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                            if (!emailSent)
                            {
                                response.Message += " (Lỗi khi gửi email)";
                            }
                        }

                        else
                        {
                            existingUser.Status = UserStatusEnum.active;
                        }
                        break;
                    
                    case AgencyStatusEnum.Active:
                        var agencyregister = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                        if (agencyregister != null)
                        {
                            var user1 = await _unitOfWork.UserRepository.GetAgencyManagerByAgencyIdAsync(agencyId);
                            user1.Status = UserStatusEnum.active;
                           await _userManager.UpdateAsync(user1);
                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "Chúc mừng! Bạn đã trở thành đối tác chính thức [futuretech-noreply]",
                                Body = $@"<p>Chào {agency.Name},</p>
                      <p>Chúng tôi rất vui mừng thông báo rằng bạn đã chính thức trở thành đối tác của Futuretech!</p>
                      <p>Đây là một bước quan trọng trong sự hợp tác giữa chúng ta, và chúng tôi rất mong được làm việc cùng bạn để đạt được những thành công to lớn.</p>
                      <p>Với tư cách là đối tác chính thức, bạn sẽ được tiếp cận với các nguồn lực, hỗ trợ và cơ hội độc quyền từ Futuretech.</p>
                      <p>Nếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, đừng ngần ngại liên hệ với chúng tôi.</p>
                      <p>Một lần nữa, xin chúc mừng và chào đón bạn đến với Futuretech!</p>
                      <p>Trân trọng,</p>
                      <p>Đội ngũ Futuretech</p>"
                            };
                            bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                            if (!emailSent)
                            {
                                response.Message += " (Lỗi khi gửi email)";
                            }
                        }

                        break;

                    case AgencyStatusEnum.Suspended:
                        var user = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agencyId);
                        if (user != null)
                        {
                            var notification = new Notification
                            {

                                CreationDate = DateTime.Now,
                                IsRead = false,
                                SenderId = agencyId.ToString(),
                                ReceiverId = user.Id,
                                Message = "Tạm ngưng hoạt động!"
                            };
                            user.Status = UserStatusEnum.inactive;
                            await _hubContext.Clients.User(agencyId.ToString()).SendAsync("ReceivedNotification", notification.Message);

                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "[futuretech-noreply]Hợp đồng hết hạn",
                                Body = $"<p>Chào {agency.Name},</p>" +
                                 $"<p>Chúng tôi  thông báo tới bạn !</p>" +
                                $"<p>Cơ sở của bạn đã tạm ngưng hoạt động</p>" +
                                 $"<P>Hãy liên hệ với đội ngũ nhân viên để được hỗ trợ </p>" +
                                 $"<p>Trân trọng,</p>" +
                                 $"<p>Đội ngũ Futuretech</p>"
                            };
                            bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                            if (!emailSent)
                            {
                                response.Message += " (Lỗi khi gửi email)";
                            }
                        }
                        break;

                }
                agency.Status = newStatus;
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Cập nhật trạng thái Đối tác thành công!";
                }
                else
                {
                    throw new Exception("Failed to update agency status.");
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

        public async Task<ApiResponse<IEnumerable<AgencyAddressViewModel>>> GetActiveAgencyAdresses()
        {
            var response = new ApiResponse<IEnumerable<AgencyAddressViewModel>>();
            try
            {
                var agencies = await _unitOfWork.AgencyRepository.FindAsync(x => x.Status == AgencyStatusEnum.Active);
                var agencyAddressViewModels = _mapper.Map<IEnumerable<AgencyAddressViewModel>>(agencies);
                if (agencyAddressViewModels.IsNullOrEmpty()) return ResponseHandler.Success(agencyAddressViewModels, "Không có chi nhánh nào đang hoạt động!");
                response = ResponseHandler.Success(agencyAddressViewModels, "Lấy địa chỉ các chi nhánh đang hoạt động thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<IEnumerable<AgencyAddressViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<IEnumerable<AgencyNameViewModel>>> GetAllAgencyAsync()
        {
            var response = new ApiResponse<IEnumerable<AgencyNameViewModel>>();
            try
            {
                var agencies = await _unitOfWork.AgencyRepository.FindAsync(x => x.Status != AgencyStatusEnum.Inactive);
                var agencyAddressViewModels = _mapper.Map<IEnumerable<AgencyNameViewModel>>(agencies);
                if (agencyAddressViewModels.IsNullOrEmpty()) return ResponseHandler.Success(agencyAddressViewModels, "Không có chi nhánh nào đang hoạt động!");
                response = ResponseHandler.Success(agencyAddressViewModels, "Lấy các chi nhánh đang hoạt động thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<IEnumerable<AgencyNameViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateAgencyVNPayInfoAsync(CreateAgencyVNPayInfoViewModel model)
        {
            try
            {
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(model.AgencyId);
                if (agency == null)
                {
                    return ResponseHandler.Failure<bool>("Agency not found.");
                }

                var existingVNPayInfo = await _unitOfWork.AgencyVnPayInfoRepository.GetByAgencyIdAsync(model.AgencyId);
                if (existingVNPayInfo != null)
                {
                    _unitOfWork.AgencyVnPayInfoRepository.HardRemove(existingVNPayInfo);
                }

                var vnPayInfo = new AgencyVnPayInfo
                {
                    AgencyId = model.AgencyId,
                    TmnCode = model.VnpayTmnCode,
                    HashSecret = model.VnpayHashSecret
                };

                await _unitOfWork.AgencyVnPayInfoRepository.AddAsync(vnPayInfo);
                await _unitOfWork.SaveChangeAsync();

                return ResponseHandler.Success(true, "VNPay info updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"Error updating VNPay info: {ex.Message}");
            }
        }
        public async Task<ApiResponse<AgencyVnpayInfoViewModel>> GetAgencyVnpayAsync()
        {
            try
            {
                var currentUserId = _claimsService.GetCurrentUserId.ToString();

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return ResponseHandler.Failure<AgencyVnpayInfoViewModel>("User not authenticated.");
                }
                var user = await _userManager.FindByIdAsync(currentUserId);

                if (user == null)
                {
                    return ResponseHandler.Failure<AgencyVnpayInfoViewModel>("Agency not found for the current user.");
                }
                var vnPayInfo = await _unitOfWork.AgencyVnPayInfoRepository.GetByAgencyIdAsync(user.AgencyId.Value);

                if (vnPayInfo == null)
                {
                    return ResponseHandler.Failure<AgencyVnpayInfoViewModel>("VNPay information not found for this agency.");
                }
                var viewModel = new AgencyVnpayInfoViewModel
                {
                    TmnCode = vnPayInfo.TmnCode,
                    HashSecret = vnPayInfo.HashSecret
                };

                return ResponseHandler.Success(viewModel, "VNPay information retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<AgencyVnpayInfoViewModel>($"Error retrieving VNPay information: {ex.Message}");
            }
        }
    }
}


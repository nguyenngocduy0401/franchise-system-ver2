
using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.NotificationViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AgencyService : IAgencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateAgencyViewModel> _validator;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailService _emailService;

        public AgencyService(IMapper mapper,IUnitOfWork unitOfWork,
            IClaimsService claimsService,IValidator<CreateAgencyViewModel>  validator,
            IUserService userService,IHubContext<NotificationHub>hubContext,
            IEmailService emailService)
        {
            _unitOfWork= unitOfWork;
            _validator= validator;
            _claimsService= claimsService;
            _mapper = mapper;
            _userService= userService;
            _hubContext= hubContext;
            _emailService= emailService;

        }

        public async  Task<ApiResponse<bool>> CreateAgencyAsync(CreateAgencyViewModel create)
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
                var agency = _mapper.Map<Agency>(create);
                agency.Status = AgencyStatusEnum.Processing;
                await _unitOfWork.AgencyRepository.AddAsync(agency);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    response.Data=true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
                    var emailMessage = new MessageModel
                    {
                        To = create.Email, 
                        Subject = "Đăng ký thành công  [futuretech-noreply]",
                        Body = $"<p>Chào {create.Name},</p>" +
                      $"<p>Cảm ơn bạn đã đăng ký trở thành đối tác của chúng tôi.</p>" +
                      $"<p>Thông tin của bạn đã được ghi nhận thành công.</p>" +
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
                    (!filter.Status.HasValue || s.Status == filter.Status) ,
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

        public async Task<ApiResponse<bool>> UpdateAgencyAsync(CreateAgencyViewModel update, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var agenyId = Guid.Parse(id);
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(update);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
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
                var notification = new Notification
                {
                    CreatedBy = _claimsService.GetCurrentUserId,
                    CreationDate = DateTime.Now,
                    IsRead=false,
                    ReceiverId=_claimsService.GetCurrentUserId.ToString(),
                };
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
                    case AgencyStatusEnum.Partner:
                        var existingUser = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agencyId);
                        if (existingUser == null)
                        {
                            var userCredentials = await _userService.GenerateUserCredentials(agency.Name);

                            var newUser = new User
                            {
                                UserName = userCredentials.UserName,
                                 PasswordHash= userCredentials.Password,
                                AgencyId = agencyId,
                                Status = UserStatusEnum.active,
                                Email=agency.Email,
                                PhoneNumber=agency.PhoneNumber,
                                

                            };
                          var user2=  _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(newUser, RolesEnum.AgencyManager.ToString());
                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "[futuretech-noreply] Chào Mừng Đối Tác Chính Thức",
                                Body = $"<p>Chào {agency.Name},</p>" +
                                   $"<p>Chúng tôi rất vui mừng thông báo rằng bạn đã trở thành đối tác chính thức của chúng tôi!</p>" +
                                   $"<p>Chúng tôi tin tưởng rằng sự hợp tác này sẽ mang lại lợi ích to lớn cho cả hai bên và chúng tôi mong muốn cùng nhau phát triển mạnh mẽ trong thời gian tới.</p>" +
                                   $"<p>Đội ngũ nhân viên hổ trợ sẻ sớm  liên hệ với bạn .</p>" +
                                    $"<p>Đây là tài khoản để bạn có thể đăng nhập hệ thống:</p>" +
                                   $"<ul>" +
                                   $"<li><strong>Username:</strong> {newUser.UserName}</li>" +
                                   $"<li><strong>Password:</strong> {userCredentials.Password}</li>" +
                                   $"</ul>" +
                                   $"<p>Vui lòng bảo mật thông tin đăng nhập này.</p>"+
                                   $"<p>Chúc mừng và hẹn gặp lại!</p>" +
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
                    case AgencyStatusEnum.Processing:
                        var agencyregister = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                        if(agencyregister != null)
                        {
                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "Đăng ký thành công  [futuretech-noreply]",
                                Body = $"<p>Chào {agency.Name},</p>" +
                                 $"<p>Thông tin của bạn đang được xử lý và chúng tôi sẽ liên hệ với bạn sớm nhất có thể.</p>" +
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

                    case AgencyStatusEnum.Terminated:
                        var user = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agencyId);
                        if (user != null)
                        {
                            var notification = new Notification
                            {
                               
                                CreationDate = DateTime.Now,
                                IsRead = false,
                                SenderId = agencyId.ToString(),
                                ReceiverId = user.Id,
                                Message = "Chấm dứt hợp đồng"
                            };
                            user.Status = UserStatusEnum.blocked;
                            await _hubContext.Clients.User(agencyId.ToString()).SendAsync("ReceivedNotification",notification.Message);

                            var emailMessage = new MessageModel
                            {
                                To = agency.Email,
                                Subject = "[futuretech-noreply]Hợp đồng hết hạn",
                                Body = $"<p>Chào {agency.Name},</p>" +
                                 $"<p>Chúng tôi  thông báo tới bạn đã hết hạn hợp đồng!</p>" +
                                
                                 $"<P>Hãy liên hệ với đội ngũ nhân viên để được hỗ trợ </p>"+
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

    }
}

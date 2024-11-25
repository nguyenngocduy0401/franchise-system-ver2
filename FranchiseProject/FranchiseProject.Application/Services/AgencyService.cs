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

        public AgencyService(IMapper mapper, IUnitOfWork unitOfWork,
            IClaimsService claimsService, IValidator<CreateAgencyViewModel> validator,
            IUserService userService, IHubContext<NotificationHub> hubContext,
            IEmailService emailService, IValidator<UpdateAgencyViewModel> validatorUpdate,
            ICurrentTime currentTime)
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
        }

        public async Task<ApiResponse<bool>> CreateAgencyAsync(CreateAgencyViewModel create)
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
                var work = CreateWorkAppointForFranchiseFlow(agency);
                agency.Works = work;
                await _unitOfWork.AgencyRepository.AddAsync(agency);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
                if (isSuccess > 0)
                {
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
                    var emailMessage = EmailTemplate.AgencyRegistrationSuccess(agency.Email, agency.Name);
                    var emailSent = _emailService.SendEmailAsync(emailMessage);
                    /*if (!emailSent)
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
                    (!filter.Activity.HasValue || s.ActivityStatus == filter.Activity) &&
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
                    IsRead = false,
                    ReceiverId = _claimsService.GetCurrentUserId.ToString(),
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
                    case AgencyStatusEnum.Approved:
                        var existingUser = await _unitOfWork.UserRepository.GetByAgencyIdAsync(agencyId);
                        if (existingUser == null)
                        {
                            var userCredentials = await _userService.GenerateUserCredentials(agency.Name);

                            var newUser = new User
                            {
                                UserName = userCredentials.UserName,
                                PasswordHash = userCredentials.Password,
                                AgencyId = agencyId,
                            //    Status = UserStatusEnum.active,
                                Email=agency.Email,
                                PhoneNumber=agency.PhoneNumber,
 
                            };
                            var user2 = _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(newUser, RolesEnum.AgencyManager.ToString());
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
                                   $"<p>Vui lòng bảo mật thông tin đăng nhập này.</p>" +
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
                    case AgencyStatusEnum.Active:
                        var agencyregister = await _unitOfWork.AgencyRepository.GetByIdAsync(agencyId);
                        if (agencyregister != null)
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
                            user.Status = UserStatusEnum.blocked;
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
        private List<Work> CreateWorkAppointForFranchiseFlow(Agency agency)
        {
            var works = new List<Work>();
            works.Add(new Work
            {
                Title = "Phỏng vấn đối tác - " + agency.Name,
                Description = "<p>Buổi phỏng vấn cần tập trung vào" +
                " các nội dung chính sau:</p>\r\n\r\n<ul>\r\n    " +
                "<li><strong>Năng lực tài chính:</strong> Xác minh khả năng tài chính của đối tác để đảm bảo họ đủ tiềm lực tham gia vào mô hình nhượng quyền.</li>\r\n   " +
                " <li><strong>Kinh nghiệm quản lý:</strong> Tìm hiểu quá trình làm việc, các dự án đã từng tham gia, và năng lực quản lý hiện tại của đối tác.</li>\r\n    " +
                "<li><strong>Mục tiêu phát triển:</strong> Trao đổi về định hướng chiến lược và kỳ vọng của đối tác khi tham gia vào mô hình nhượng quyền của công ty.</li>\r\n  " +
                "  <li><strong>Cam kết hợp tác:</strong> Đảm bảo đối tác hiểu và sẵn sàng tuân thủ các quy định, tiêu chuẩn, và lộ trình mà công ty đã đề ra.</li>\r\n</ul>\r\n\r\n<p>Nhân viên cần chuẩn bị trước các tài liệu sau:</p>\r\n<ul>\r\n    " +
                "<li>Tài liệu giới thiệu công ty</li>\r\n    " +
                "<li>Bộ câu hỏi phỏng vấn</li>\r\n   " +
                " <li>Biểu mẫu ghi chú hoặc checklist đánh giá</li>\r\n</ul>\r\n\r\n<p>Buổi phỏng vấn phải đảm bảo tính <strong>chuyên nghiệp</strong>, <strong>khách quan</strong>, và <strong>minh bạch</strong>," +
                " nhằm tạo ấn tượng tốt với đối tác. Sau buổi phỏng vấn, hãy lập báo cáo chi tiết về nội dung đã trao đổi, bao gồm:</p>\r\n<ul>\r\n  " +
                "  <li>Đánh giá sơ bộ về năng lực tài chính</li>\r\n    <li>Kinh nghiệm quản lý và điều hành</li>\r\n    " +
                "<li>Mức độ phù hợp với quy trình nhượng quyền</li>\r\n</ul>\r\n\r\n<p><em>Thời gian thực hiện:</em> 1-2 giờ</p>\r\n<p><em>Kết quả mong đợi:</em> Báo cáo chi tiết trình quản lý để xem xét và phê duyệt.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.Interview,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime(),
                AgencyId = agency.Id,
                Appointments = new List<Appointment> { new Appointment
                {
                    Title = "Phỏng vấn đối tác - " + agency.Name,
                    StartTime = _currentTime.GetCurrentTime(),
                    EndTime = _currentTime.GetCurrentTime(),
                    Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Địa chỉ công ty hoặc cuộc họp trực tuyến]</li>\r\n</ul>",
                    Status = AppointmentStatusEnum.None,
                    Type = AppointmentTypeEnum.WithAgency,

                }}
            });
            works.Add(new Work
            {
                Title = "Kí hợp đồng với đối tác - " + agency.Name,
                Description = "<p><strong>Ký Thỏa Thuận Hợp Đồng</strong> giữa công ty và đối tác là bước quan trọng trong quá trình bắt đầu hợp tác kinh doanh. Buổi ký kết hợp đồng sẽ diễn ra sau khi tất cả các điều khoản và điều kiện của hợp đồng đã được thống nhất giữa hai bên.</p>\r\n\r\n<ul>\r\n " +
                "   <li><strong>Kiểm tra và hoàn thiện hợp đồng:</strong> Đảm bảo rằng tất cả các điều khoản trong hợp đồng đều rõ ràng, minh bạch và đã được xem xét kỹ lưỡng bởi cả hai bên.</li>\r\n    <li><strong>Ký kết hợp đồng:</strong> Đại diện của công ty và đối tác sẽ ký vào các bản hợp đồng, xác nhận sự đồng ý về các điều khoản đã được thảo luận và thống nhất.</li>\r\n    <li><strong>Thông báo hoàn tất:</strong> Sau khi hợp đồng đã được ký, các bên sẽ nhận thông báo chính thức về việc hợp tác" +
                " chính thức và bắt đầu triển khai các bước tiếp theo trong kế hoạch hợp tác.</li>\r\n</ul>\r\n\r\n<p><em>Thời gian thực hiện:</em> 1-2 giờ</p>\r\n" +
                "<p><em>Kết quả mong đợi:</em> Hợp đồng chính thức được ký kết, các bước tiếp theo trong hợp tác sẽ được triển khai.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.SignedContract,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime(),
                AgencyId = agency.Id,
                Appointments = new List<Appointment> { new Appointment
                {
                    Title = "Kí hợp đồng với đối tác - " + agency.Name,
                    StartTime = _currentTime.GetCurrentTime(),
                    EndTime = _currentTime.GetCurrentTime(),
                    Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Địa chỉ công ty hoặc cuộc họp trực tuyến]</li>\r\n    <li><strong>Tài liệu cần chuẩn bị:</strong> [Tài liệu của khách hàng hoặc nhân viên cần chuẩn bị]</li>\r\n    <li><strong>Mục đích của cuộc phỏng vấn:</strong> [Mục đích cụ thể của buổi phỏng vấn]</li>\r\n</ul>",
                    Status = AppointmentStatusEnum.None,
                    Type = AppointmentTypeEnum.WithAgency,

                }}
            });
            // BusinessRegistered - Đăng ký doanh nghiệp
            works.Add(new Work
            {
                Title = "Đăng ký doanh nghiệp - " + agency.Name,
                Description = "<p>Thực hiện đăng ký doanh nghiệp để hoàn tất thủ tục pháp lý trước khi bắt đầu hoạt động kinh doanh.</p>\r\n<ul>\r\n" +
                "    <li><strong>Chuẩn bị hồ sơ:</strong> Bao gồm giấy tờ pháp lý, đơn đăng ký và các tài liệu liên quan.</li>\r\n" +
                "    <li><strong>Nộp hồ sơ:</strong> Tại cơ quan đăng ký kinh doanh hoặc trực tuyến.</li>\r\n" +
                "    <li><strong>Nhận giấy phép:</strong> Sau khi hồ sơ được xét duyệt.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 3-5 ngày làm việc.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.BusinessRegistered,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(5),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
            {
                new Appointment
                {
                    Title = "Đăng ký doanh nghiệp - " + agency.Name,
                    StartTime = _currentTime.GetCurrentTime(),
                    EndTime = _currentTime.GetCurrentTime().AddHours(2),
                    Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Cơ quan đăng ký hoặc trực tuyến]</li>\r\n</ul>",
                    Status = AppointmentStatusEnum.None,
                    Type = AppointmentTypeEnum.WithAgency
                }
            }
            });

            // SiteSurvey - Khảo sát mặt bằng
            works.Add(new Work
            {
                Title = "Khảo sát mặt bằng - " + agency.Name,
                Description = "<p>Khảo sát mặt bằng để đảm bảo địa điểm đáp ứng các yêu cầu kinh doanh.</p>\r\n<ul>\r\n" +
                "    <li><strong>Đo đạc và phân tích:</strong> Xác định kích thước, vị trí và các yếu tố địa lý.</li>\r\n" +
                "    <li><strong>Đánh giá tiềm năng:</strong> Phân tích lưu lượng khách hàng và điều kiện kinh doanh tại khu vực.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 1-2 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.SiteSurvey,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(2),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        Title = "Khảo sát mặt bằng - " + agency.Name,
                        StartTime = _currentTime.GetCurrentTime(),
                        EndTime = _currentTime.GetCurrentTime().AddHours(3),
                        Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Vị trí mặt bằng]</li>\r\n</ul>",
                        Status = AppointmentStatusEnum.None,
                        Type = AppointmentTypeEnum.WithAgency
                    }
                }
            });

            // Design - Thiết kế
            works.Add(new Work
            {
                Title = "Thiết kế mặt bằng - " + agency.Name,
                Description = "<p>Lên bản thiết kế chi tiết cho mặt bằng kinh doanh.</p>\r\n<ul>\r\n" +
                "    <li><strong>Tiếp nhận yêu cầu:</strong> Xác định nhu cầu của đối tác.</li>\r\n" +
                "    <li><strong>Thiết kế sơ bộ:</strong> Đưa ra các phương án thiết kế phù hợp.</li>\r\n" +
                "    <li><strong>Phê duyệt:</strong> Lấy ý kiến đối tác và hoàn thiện bản thiết kế.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 5-7 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.Design,
                Level = WorkLevelEnum.Optional,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(7),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        Title = "Thiết kế mặt bằng - " + agency.Name,
                        StartTime = _currentTime.GetCurrentTime(),
                        EndTime = _currentTime.GetCurrentTime().AddHours(2),
                        Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Văn phòng hoặc trực tuyến]</li>\r\n</ul>",
                        Status = AppointmentStatusEnum.None,
                        Type = AppointmentTypeEnum.WithAgency
                    }
                }
            });
            // Quotation - Báo giá cho khách hàng
            works.Add(new Work
            {
                Title = "Báo giá cho khách hàng - " + agency.Name,
                Description = "<p>Chuẩn bị và gửi bảng báo giá chi tiết cho đối tác.</p>\r\n<ul>\r\n" +
                "    <li><strong>Xác định chi phí:</strong> Tính toán chi phí dựa trên các yêu cầu cụ thể của đối tác.</li>\r\n" +
                "    <li><strong>Lập bảng báo giá:</strong> Bao gồm các mục chi phí cụ thể, thời gian hoàn thành và điều khoản thanh toán.</li>\r\n" +
                "    <li><strong>Gửi và thảo luận:</strong> Trình bày và giải đáp thắc mắc của đối tác về báo giá.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 2-3 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.Quotation,
                Level = WorkLevelEnum.Optional,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(3),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
    {
        new Appointment
        {
            Title = "Báo giá cho khách hàng - " + agency.Name,
            StartTime = _currentTime.GetCurrentTime(),
            EndTime = _currentTime.GetCurrentTime().AddHours(2),
            Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Trực tuyến hoặc văn phòng]</li>\r\n</ul>",
            Status = AppointmentStatusEnum.None,
            Type = AppointmentTypeEnum.WithAgency
        }
    }
            });

            // SignedContract - Ký hợp đồng thành công
            works.Add(new Work
            {
                Title = "Ký hợp đồng thành công - " + agency.Name,
                Description = "<p>Ký kết hợp đồng chính thức để hoàn tất thỏa thuận hợp tác.</p>\r\n<ul>\r\n" +
                "    <li><strong>Rà soát hợp đồng:</strong> Đảm bảo các điều khoản đã được hai bên đồng thuận.</li>\r\n" +
                "    <li><strong>Ký hợp đồng:</strong> Đại diện hai bên ký xác nhận thỏa thuận hợp tác.</li>\r\n" +
                "    <li><strong>Thông báo kết quả:</strong> Thông báo chính thức việc ký kết và khởi động dự án.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 1 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.SignedContract,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(1),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
    {
        new Appointment
        {
            Title = "Ký hợp đồng thành công - " + agency.Name,
            StartTime = _currentTime.GetCurrentTime(),
            EndTime = _currentTime.GetCurrentTime().AddHours(2),
            Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Văn phòng hoặc địa điểm ký kết]</li>\r\n</ul>",
            Status = AppointmentStatusEnum.None,
            Type = AppointmentTypeEnum.WithAgency
        }
    }
            });

            // ConstructionAndTraining - Đào tạo và thi công
            works.Add(new Work
            {
                Title = "Đào tạo và thi công - " + agency.Name,
                Description = "<p>Tiến hành thi công và đào tạo đối tác để đảm bảo vận hành hiệu quả.</p>\r\n<ul>\r\n" +
                "    <li><strong>Thi công cơ sở:</strong> Lắp đặt và bố trí nội thất, trang thiết bị.</li>\r\n" +
                "    <li><strong>Đào tạo:</strong> Hướng dẫn quy trình vận hành, quản lý và kỹ năng cần thiết.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 10-15 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.ConstructionAndTrainning,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(15),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        Title = "Đào tạo và thi công - " + agency.Name,
                        StartTime = _currentTime.GetCurrentTime(),
                        EndTime = _currentTime.GetCurrentTime().AddHours(6),
                        Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Cơ sở kinh doanh]</li>\r\n</ul>",
                        Status = AppointmentStatusEnum.None,
                        Type = AppointmentTypeEnum.WithAgency
                    }
                }
            });

            // Handover - Bàn giao
            works.Add(new Work
            {
                Title = "Bàn giao cơ sở - " + agency.Name,
                Description = "<p>Hoàn tất quá trình thi công và bàn giao cơ sở kinh doanh.</p>\r\n<ul>\r\n" +
                "    <li><strong>Kiểm tra hoàn tất:</strong> Đảm bảo cơ sở và trang thiết bị đã sẵn sàng hoạt động.</li>\r\n" +
                "    <li><strong>Bàn giao:</strong> Chuyển giao tài liệu và hướng dẫn vận hành.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 1 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.Handover,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(1),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        Title = "Bàn giao cơ sở - " + agency.Name,
                        StartTime = _currentTime.GetCurrentTime(),
                        EndTime = _currentTime.GetCurrentTime().AddHours(2),
                        Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Cơ sở kinh doanh]</li>\r\n</ul>",
                        Status = AppointmentStatusEnum.None,
                        Type = AppointmentTypeEnum.WithAgency
                    }
                }
            });

            // EducationLicenseRegistered - Đăng ký giấy phép giáo dục
            works.Add(new Work
            {
                Title = "Đăng ký giấy phép giáo dục - " + agency.Name,
                Description = "<p>Hoàn tất đăng ký giấy phép giáo dục để vận hành cơ sở kinh doanh.</p>\r\n<ul>\r\n" +
                "    <li><strong>Chuẩn bị hồ sơ:</strong> Bao gồm các tài liệu cần thiết theo quy định pháp luật.</li>\r\n" +
                "    <li><strong>Nộp hồ sơ:</strong> Tại cơ quan quản lý giáo dục địa phương.</li>\r\n" +
                "    <li><strong>Nhận giấy phép:</strong> Sau khi hồ sơ được phê duyệt.</li>\r\n</ul>\r\n<p><em>Thời gian thực hiện:</em> 7-10 ngày.</p>",
                Status = WorkStatusEnum.None,
                Submit = WorkStatusSubmitEnum.None,
                Type = WorkTypeEnum.EducationLicenseRegistered,
                Level = WorkLevelEnum.Compulsory,
                StartDate = _currentTime.GetCurrentTime(),
                EndDate = _currentTime.GetCurrentTime().AddDays(10),
                AgencyId = agency.Id,
                Appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        Title = "Đăng ký giấy phép giáo dục - " + agency.Name,
                        StartTime = _currentTime.GetCurrentTime(),
                        EndTime = _currentTime.GetCurrentTime().AddHours(3),
                        Description = "<ul>\r\n    <li><strong>Thời gian:</strong> [Ngày, Giờ]</li>\r\n    <li><strong>Địa điểm:</strong> [Cơ quan quản lý giáo dục]</li>\r\n</ul>",
                        Status = AppointmentStatusEnum.None,
                        Type = AppointmentTypeEnum.WithAgency
                    }
                }
            });
            return works;
        }
    }
}

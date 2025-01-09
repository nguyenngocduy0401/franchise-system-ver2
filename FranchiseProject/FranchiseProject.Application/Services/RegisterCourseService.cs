using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.NotificationViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Application.ViewModels.VnPayViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class RegisterCourseService : IRegisterCourseService
    {
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterCourseViewModel> _validator;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IValidator<UpdateRegisterCourseViewModel> _updateValidator;
        private readonly ICurrentTime _currentTime;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly INotificationService _notificationService;
        public RegisterCourseService(IValidator<UpdateRegisterCourseViewModel> updateValidator, RoleManager<Role> roleManager,
            IEmailService emailService, IClaimsService claimsService,
            UserManager<User> userManager, IMapper mapper,
            IUnitOfWork unitOfWork, IValidator<RegisterCourseViewModel> validator,
            ICurrentTime currentTime, IUserService userService, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory,
            INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userManager = userManager;
            _claimsService = claimsService;
            _emailService = emailService;
            _roleManager = roleManager;
            _updateValidator = updateValidator;
            _currentTime = currentTime;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<bool>> RequestRefundByCourseIdAsync(Guid courseId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var studentId = _claimsService.GetCurrentUserId.ToString();
                var registerCourse = (await _unitOfWork.RegisterCourseRepository
                    .FindAsync(e => e.UserId == studentId && e.CourseId == courseId))
                    .OrderByDescending(e => e.DateTime)
                    .FirstOrDefault();
                if (registerCourse == null) return ResponseHandler.Success(false, "Khoá học hiện không khả dụng!");
                if (registerCourse.StudentPaymentStatus == StudentPaymentStatusEnum.RequestRefund) return ResponseHandler.Success(false, "Đã yêu cầu hoàn tiền trước đó!");

                registerCourse.StudentPaymentStatus = StudentPaymentStatusEnum.RequestRefund;
                _unitOfWork.RegisterCourseRepository.Update(registerCourse);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update fail!");
                return ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>(ex.Message);
            }
        }
        public async Task<ApiResponse<string>> RegisterCourseAsync(RegisterCourseViewModel model)
        {
            var response = new ApiResponse<string>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<string>(validationResult);
                }

                var twentyFourHoursAgo = DateTime.Now.AddHours(-24);
                bool existsWithin24Hours = await _unitOfWork.RegisterCourseRepository.ExistsWithinLast24HoursAsync(model.StudentName, model.Email, model.PhoneNumber, model.CourseId);

                if (existsWithin24Hours)
                {
                    return ResponseHandler.Success<string>(null, "Bạn đã đăng ký khóa học này trong vòng 24 giờ qua. Vui lòng thử lại sau.");
                }

                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(Guid.Parse(model.CourseId));
                if (course == null) return ResponseHandler.Success<string>(null, "Khóa học không khả dụng!");

                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(Guid.Parse(model.AgencyId));
                if (agency == null) return ResponseHandler.Success<string>(null, "Trung tâm không khả dụng!");

                // Tạo tài khoản người dùng
                var newUser = new User
                {


                    FullName = model.StudentName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AgencyId = Guid.Parse(model.AgencyId),
                    Status = UserStatusEnum.active,
                    CreateAt = _currentTime.GetCurrentTime(),
                };
                var generate = await _userService.GenerateUserCredentials(newUser.FullName);
                newUser.UserName = generate.UserName;
                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    return ResponseHandler.Failure<string>("Không thể tạo tài khoản người dùng.");
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangeAsync();
                var newRegisterCourse = new RegisterCourse
                {
                    UserId = newUser.Id,
                    CourseId = course.Id,
                    StudentCourseStatus = StudentCourseStatusEnum.NotConsult,
                    Email = newUser.Email,
                    StudentPaymentStatus = StudentPaymentStatusEnum.Pending_Payment

                };
                await _unitOfWork.RegisterCourseRepository.AddAsync(newRegisterCourse);


                // Tạo URL thanh toán VnPay
                var paymentViewModel = new AgencyCoursePaymentViewModel
                {
                    CourseId = Guid.Parse(model.CourseId),
                    UserId = newUser.Id,
                    AgencyId = Guid.Parse(model.AgencyId)
                    ,
                    RegisterCourseId = newRegisterCourse.Id
                };
                var vnPayService = _serviceProvider.GetRequiredService<IVnPayService>();



                // Lưu thông tin tạm thời
                var tempRegistration = new TempRegistrations
                {
                    UserId = newUser.Id,
                    CourseId = Guid.Parse(model.CourseId),
                    ClassId = model.ClassId.Value,
                    CreationDate = DateTime.Now
                };
                await _unitOfWork.TempRegistrationsRepository.AddAsync(tempRegistration);
                await _unitOfWork.SaveChangeAsync();
                string paymentUrl = await vnPayService.CreatePaymentUrlForAgencyCourse(paymentViewModel);
                return ResponseHandler.Success(paymentUrl, "Vui lòng tiến hành thanh toán để hoàn tất đăng ký.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>(ex.Message);
            }
        }
        public async Task<ApiResponse<bool>> CompleteRegistrationAfterPayment(string userId, Guid registerCourseId, Guid paymentId)
        {
            try
            {
                var rc = await _unitOfWork.RegisterCourseRepository.GetExistByIdAsync(registerCourseId);
                var user = await _userManager.FindByIdAsync(userId);
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(rc.CourseId.Value);
                var tempRegistration = await _unitOfWork.TempRegistrationsRepository.GetByUserIdAndCourseIdAsync(userId, course.Id);
                var class1 = await _unitOfWork.ClassRepository.GetExistByIdAsync(tempRegistration.ClassId);

                if (user == null || course == null || tempRegistration == null)
                {
                    return ResponseHandler.Failure<bool>("Không tìm thấy thông tin đăng ký.");
                }

                // Thêm vào lớp
                var classScheduleEarliest = await _unitOfWork.ClassScheduleRepository.GetEarliestClassScheduleByClassIdAsync(tempRegistration.ClassId);
                var classScheduleLatest = await _unitOfWork.ClassScheduleRepository.GetLatestClassScheduleByClassIdAsync(tempRegistration.ClassId);
                var classRoom = new ClassRoom
                {
                    ClassId = tempRegistration.ClassId,
                    UserId = userId,
                    FromDate = classScheduleEarliest?.Date != null ? DateOnly.FromDateTime(classScheduleEarliest.Date.Value) : null,
                    ToDate = classScheduleLatest?.Date != null ? DateOnly.FromDateTime(classScheduleLatest.Date.Value) : null,
                };
                await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);

                //gắn attendance
                var classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == tempRegistration.ClassId);
                foreach (var classSchedule in classSchedules)
                {
                    var attendance = new Attendance
                    {
                        UserId = userId,
                        ClassScheduleId = classSchedule.Id,
                        Status = AttendanceStatusEnum.NotStarted
                    };
                    await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                }

                bool isNewAccount = false;
                string generatedPassword = null;

                // Kiểm tra xem người dùng đã có vai trò Student chưa
                if (!await _userManager.IsInRoleAsync(user, AppRole.Student))
                {
                    // Nếu chưa có vai trò Student, tạo thông tin đăng nhập mới
                    var generate = await _userService.GenerateUserCredentials(user.FullName);
                    user.UserName = generate.UserName;
                    await _userManager.AddToRoleAsync(user, AppRole.Student);
                    await _userManager.AddPasswordAsync(user, generate.Password);
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new Exception("Cập nhật tài khoản người dùng thất bại!");
                    }

                    isNewAccount = true;
                    generatedPassword = generate.Password;
                }

                //cap nhat thanh toan
                rc.StudentPaymentStatus = StudentPaymentStatusEnum.Completed;
                rc.UserId = user.Id;
                var payment = await _unitOfWork.PaymentRepository.GetExistByIdAsync(paymentId);
                payment.UserId = user.Id;
                payment.ToDate = classRoom.ToDate;
                class1.CurrentEnrollment++;
                _unitOfWork.PaymentRepository.Update(payment);
                _unitOfWork.RegisterCourseRepository.Update(rc);
                //cập nhật số account 
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(user.AgencyId.Value);
                contract.UsedAccountCount++;
                _unitOfWork.ContractRepository.Update(contract);
              
                // Gửi email
                var startDate = classScheduleEarliest.Date.Value.ToString("dd/MM/yyyy");
                var endDate = classScheduleLatest.Date.Value.ToString("dd/MM/yyyy");
                var slot = await _unitOfWork.SlotRepository.GetExistByIdAsync((Guid)classSchedules.FirstOrDefault().SlotId);

                Dictionary<string, string> dayOfWeekMap = new Dictionary<string, string>
                {
                    { "Sunday", "Chủ nhật" },
                    { "Monday", "Thứ hai" },
                    { "Tuesday", "Thứ ba" },
                    { "Wednesday", "Thứ tư" },
                    { "Thursday", "Thứ năm" },
                    { "Friday", "Thứ sáu" },
                    { "Saturday", "Thứ bảy" }
                };
                //string vietnameseDayOfWeek = dayOfWeekMap.ContainsKey(class1.DayOfWeek) ? dayOfWeekMap[class1.DayOfWeek] : "None";
                string vietnameseDayOfWeek = string.Join(", ",
                class1.DayOfWeek.Split(',').Select(day =>
                    dayOfWeekMap.ContainsKey(day.Trim()) ? dayOfWeekMap[day.Trim()] : "None"
                )
            );
                var studentDayOfWeek = vietnameseDayOfWeek + "-" + slot?.StartTime + "-" + slot?.EndTime;

                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(user.AgencyId.Value);
                var address = agency.Address + ", " + agency.Ward + ", " + agency.District + ", " + agency.City;

               MessageModel  emailMessage;
                if (isNewAccount)
                {
                     emailMessage = EmailTemplate.SuccessRegisterCourseEmaill(user.Email, user.FullName, course.Name, user.UserName, generatedPassword,
                        (decimal)course.Price, studentDayOfWeek, startDate, endDate, address);
                }
                else
                {
                    emailMessage = EmailTemplate.SuccessRegisterCourseEmailWithoutCredentials(user.Email, user.FullName, course.Name, user.UserName, generatedPassword,
                        (decimal)course.Price, studentDayOfWeek, startDate, endDate, address);
                }

                bool emailSent = await _emailService.SendEmailAsync(emailMessage);

                //Gui noti 
                var agencyUserIds = await _unitOfWork.UserRepository.GetAgencyUsersAsync(user.AgencyId.Value);
                if (agencyUserIds != null && agencyUserIds.Any())
                {
                    var noti = new SendNotificationViewModel
                    {
                        message = $@"Một học viên mới đã đăng ký khóa học {course.Name} thành công!",
                        userIds = agencyUserIds
                    };
                    await _notificationService.CreateAndSendNotificationNoReponseForRegisterAsync(noti);
                    await _unitOfWork.SaveChangeAsync();
                }

                // Xóa thông tin tạm thời
                _unitOfWork.TempRegistrationsRepository.HardRemove(tempRegistration);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Hoàn tất đăng ký thất bại!");

                return ResponseHandler.Success(true, "Đăng ký thành công!");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>(ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusStudentAsync(string studentId, string courseId, StudentCourseStatusEnum status)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var student = await _userManager.FindByIdAsync(studentId);
                if (student == null)
                {
                    return ResponseHandler.Success<bool>(false, "Học sinh không khả dụng!");
                }
                var courseGuidId = Guid.Parse(courseId);
                var registerCourse = await _unitOfWork.RegisterCourseRepository
                    .GetFirstOrDefaultAsync(rc => rc.UserId == studentId && rc.CourseId == courseGuidId);
                //  var userCurrent = await _userManager.FindByIdAsync(_claimsService.GetCurrentUserId.ToString());
                if (registerCourse == null)
                {
                    return ResponseHandler.Success<bool>(false, "Không tìm thấy bản ghi khóa học của học sinh!");
                }
                switch (status)
                {
                    case StudentCourseStatusEnum.Pending:
                        if (registerCourse.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        {
                            registerCourse.StudentPaymentStatus = StudentPaymentStatusEnum.Pending_Payment;
                            registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Pending;
                        }
                        else
                        {
                            return ResponseHandler.Success<bool>(false, "Học sinh không thể chuyển thành trạng thái Chờ!");
                        }
                        break;

                    case StudentCourseStatusEnum.Cancel:
                        registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Cancel;
                        registerCourse.StudentPaymentStatus = StudentPaymentStatusEnum.Late_Payment;
                        break;
                    case StudentCourseStatusEnum.Waitlisted:
                        registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;
                        break;
                }
                registerCourse.ConsultanId = _claimsService.GetCurrentUserId.ToString();
                registerCourse.ModificationDate = DateTime.Now;
                await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                await _unitOfWork.SaveChangeAsync();

                response = ResponseHandler.Success(true, "Cập nhật trạng thái học sinh thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }

            return response;
        }

        public async Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync(string id, string courseId)
        {
            var response = new ApiResponse<StudentRegisterViewModel>();

            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                var student = await _userManager.FindByIdAsync(id);
                if (student == null)
                {
                    return ResponseHandler.Success<StudentRegisterViewModel>(null, "Học sinh không tồn tại!");
                }

                if (student.AgencyId != userCurrent.AgencyId)
                {
                    return ResponseHandler.Success<StudentRegisterViewModel>(null, "Học sinh không thuộc về agency của bạn!");
                }
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc =>
                    rc.UserId == id &&
                    rc.CourseId == Guid.Parse(courseId)
                    );
                if (registerCourse == null)
                {
                    return ResponseHandler.Success<StudentRegisterViewModel>(null, "Học sinh chưa đăng ký khóa học này!");
                }
                var rc = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc => rc.UserId == id && rc.CourseId == Guid.Parse(courseId));
                var courseCodes = await _unitOfWork.RegisterCourseRepository.GetCourseCodeByUserIdAsync(id);
                string consultantName = null;
                if (!string.IsNullOrEmpty(registerCourse.ModificationBy.ToString()))
                {
                    var consultant = await _userManager.FindByIdAsync(registerCourse.ModificationBy.ToString());
                    consultantName = consultant?.UserName;
                }
                var studentViewModel = new StudentRegisterViewModel
                {
                    Id = rc.Id,
                    UserId = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    CourseCode = courseCodes.FirstOrDefault(),
                    StudentStatus = rc.StudentCourseStatus,
                    CourseId = registerCourse.CourseId,
                    DateTime = await GetDateTimeFromRegisterCourseAsync(id, registerCourse.CourseId.Value),
                    ConsultantName = consultantName,
                    CoursePrice = registerCourse.Course?.Price,
                    CreationDate = registerCourse.CreationDate,
                    ModificationDate = registerCourse.ModificationDate.ToString(),
                    PaymentStatus = registerCourse.StudentPaymentStatus,
                };

                response = ResponseHandler.Success(studentViewModel, "Lấy thông tin thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<StudentRegisterViewModel>(ex.Message);
            }

            return response;
        }
        public async Task<ApiResponse<Pagination<StudentRegisterViewModel>>> FilterStudentAsync(FilterRegisterCourseViewModel filterStudentModel)
        {
            var response = new ApiResponse<Pagination<StudentRegisterViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<Pagination<StudentRegisterViewModel>>(null, "User hoặc Agency không khả dụng!");
                }
                var allowedStatuses = new List<StudentCourseStatusEnum>
                {
                    StudentCourseStatusEnum.Pending,
                    StudentCourseStatusEnum.Waitlisted,
                    StudentCourseStatusEnum.Cancel,
                    StudentCourseStatusEnum.NotConsult,
                    StudentCourseStatusEnum.Enrolled
                };
                Expression<Func<RegisterCourse, bool>> filter = rc =>
           allowedStatuses.Contains(rc.StudentCourseStatus.Value) &&
           (filterStudentModel.Status == null || rc.StudentCourseStatus == filterStudentModel.Status) &&
           (filterStudentModel.PaymentStatus == null || rc.StudentPaymentStatus == filterStudentModel.PaymentStatus) &&
           (string.IsNullOrEmpty(filterStudentModel.CourseId) || rc.CourseId.ToString() == filterStudentModel.CourseId) &&
           rc.User.AgencyId == userCurrent.AgencyId &&
           (string.IsNullOrEmpty(filterStudentModel.SearchInput) ||
            (rc.User.FullName.Contains(filterStudentModel.SearchInput) ||
             rc.User.PhoneNumber.Contains(filterStudentModel.SearchInput)));

                var registerCourses = await _unitOfWork.RegisterCourseRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "User,Course",
                    pageIndex: filterStudentModel.PageIndex,
                    pageSize: filterStudentModel.PageSize,
                    orderBy: q => filterStudentModel.SortOrder.ToLower() == "asc"
                        ? q.OrderBy(rc => rc.CreationDate)
                        : q.OrderByDescending(rc => rc.CreationDate)
                );
                var registerCourseIds = registerCourses.Items.Select(rc => rc.Id).ToList();
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(
                    p => registerCourseIds.Contains((Guid)p.RegisterCourseId)
                );
                var paymentTotalByCourse = payments
                        .Where(p => p.Type != PaymentTypeEnum.Refund)
                    .GroupBy(p => p.RegisterCourseId )
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));
                var refundTotalByCourse = payments
                      .Where(p => p.Type == PaymentTypeEnum.Refund)
                  .GroupBy(p => p.RegisterCourseId)
                  .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));
                var refundDateByCourse = payments
    .Where(p => p.Type == PaymentTypeEnum.Refund)
    .GroupBy(p => p.RegisterCourseId)
    .ToDictionary(g => g.Key, g => g.Max(p => p.CreationDate));
                var consultantIds = registerCourses.Items
                     .Where(item => item?.ConsultanId != null)
                     .Select(item => item.ConsultanId)
                     .Distinct()
                     .ToList();

                var consultants = await _userManager.Users
                    .Where(u => consultantIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);


                var studentRegisterViewModelTasks = registerCourses.Items.OrderByDescending(item => item.CreationDate).Select(async rc =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var classInfo = await unitOfWork.ClassRoomRepository
                            .GetQueryable()
                            .Where(cr => cr.UserId == rc.UserId)
                            .Select(cr => new
                            {
                                Class = cr.Class,
                                ClassSchedules = cr.Class.ClassSchedules
                                    .OrderBy(cs => cs.Date)
                                    .ThenBy(cs => cs.Slot.StartTime)
                                    .Select(cs => new
                                    {
                                        Date = cs.Date,
                                        StartTime = cs.Slot.StartTime,
                                        EndTime = cs.Slot.EndTime,
                                    })
                                    .ToList()
                            })
                            .FirstOrDefaultAsync();

                        string classScheduleString = "";
                        DateTime? startDate = null;
                        DateTime? endDate = null;
                        string className = "";
                        if (classInfo != null && classInfo.ClassSchedules.Any())
                        {
                            className = classInfo.Class.Name;
                            var firstSchedule = classInfo.ClassSchedules.FirstOrDefault();
                            if (firstSchedule != null)
                            {
                                string dayOfWeek = classInfo.Class.DayOfWeek.ToString();
                                string timeStr = "";
                                if (firstSchedule.StartTime.HasValue && firstSchedule.EndTime.HasValue)
                                {
                                    timeStr = $"{firstSchedule.StartTime.Value:hh\\:mm}-{firstSchedule.EndTime.Value:hh\\:mm}";
                                }
                                classScheduleString = $"{dayOfWeek} {"-"} {timeStr}".Trim();
                            }
                            startDate = classInfo.ClassSchedules.Min(cs => cs.Date);
                            endDate = classInfo.ClassSchedules.Max(cs => cs.Date);
                        }

                        return new StudentRegisterViewModel
                        {
                            Id = rc.Id,
                            UserId = rc.UserId,
                            CourseId = rc.CourseId,
                            FullName = rc.User?.FullName,
                            Email = rc.User?.Email,
                            PhoneNumber = rc.User?.PhoneNumber,
                            CourseCode = rc.Course?.Code,
                            CoursePrice = rc.Course?.Price,
                            CreationDate = rc.CreationDate,
                            ModificationDate = rc.ModificationDate.ToString(),
                            ConsultantName = rc.ConsultanId != null && consultants.TryGetValue(rc.ConsultanId, out var userName)
                                ? userName
                                : null,
                            StudentStatus = rc.StudentCourseStatus,
                            PaymentStatus = rc.StudentPaymentStatus,
                            DateTime = rc.DateTime,
                            StudentAmountPaid = paymentTotalByCourse.ContainsKey(rc.Id) ? paymentTotalByCourse[rc.Id] : 0,
                            PaymentDate = rc.CreationDate,
                            ClassSchedule = classScheduleString.ToString(),
                            StartDate = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null,
                            EndDate = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null,
                            ClassName= className,
                            RefundAmount= refundTotalByCourse.ContainsKey(rc.Id) ? refundTotalByCourse[rc.Id] : 0,
                            RefundDate= refundDateByCourse.ContainsKey(rc.Id) ? refundDateByCourse[rc.Id] : null,
                        };
                    }
                }).ToList();
                var studentRegisterViewModels = await Task.WhenAll(studentRegisterViewModelTasks);
                var paginatedResult = new Pagination<StudentRegisterViewModel>
                {
                    Items =studentRegisterViewModels,
                    PageIndex = registerCourses.PageIndex,
                    PageSize = registerCourses.PageSize,
                    TotalItemsCount = registerCourses.TotalItemsCount
                };

                if (studentRegisterViewModels.Length == 0)
                {
                    return ResponseHandler.Success(paginatedResult, "Không tìm thấy học viên phù hợp!");
                }

                response = ResponseHandler.Success(paginatedResult, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<StudentRegisterViewModel>>(ex.Message);
            }
            return response;
        }
        private string GetClassScheduleString(Class classe)
        {
            if (classe?.ClassSchedules == null || !classe.ClassSchedules.Any())
                return null;

            var firstSchedule = classe.ClassSchedules.FirstOrDefault();
            if (firstSchedule?.Slot == null)
                return null;

            return $"{classe.DayOfWeek}, {firstSchedule.Slot.StartTime:hh:mm tt} - {firstSchedule.Slot.EndTime:hh:mm tt}";
        }
        private DateOnly? GetStartDate(Class classe)
        {
            return classe?.ClassSchedules?.Min(cs => DateOnly.FromDateTime(cs.Date.Value));
        }

        private DateOnly? GetEndDate(Class classe)
        {
            return classe?.ClassSchedules?.Max(cs => DateOnly.FromDateTime(cs.Date.Value));
        }
        public async Task<ApiResponse<bool>> UpdateRegisterCourseDateTimeAsync(string id, UpdateRegisterCourseViewModel update)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var rc = await _unitOfWork.RegisterCourseRepository.GetExistByIdAsync(Guid.Parse(id));
                var userId = rc.UserId;
                var user = await _userManager.FindByIdAsync(userId);
                user.FullName = update.StudentName;
                rc.DateTime = update.DateTime;
                rc.CourseId = Guid.Parse(update.CourseId);
                rc.PaymentDeadline = update.PaymentDeadline;
                await _userManager.UpdateAsync(user);
                await _unitOfWork.RegisterCourseRepository.UpdateAsync(rc);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (isSuccess)
                {
                    return ResponseHandler.Success<bool>(false, "Update Fail!");
                }


                response = ResponseHandler.Success(true, "Cập nhật  thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<string>> StudentExistRegisterCourse(string classId)
        {
            var response = new ApiResponse<string>();
            try
            {
                var classGuidId = Guid.Parse(classId);
                var studentId = _claimsService.GetCurrentUserId.ToString();
                var student = await _userManager.FindByIdAsync(studentId);

                // Lấy thông tin lớp học
                var classInfo = await _unitOfWork.ClassRepository.GetExistByIdAsync(classGuidId);
                if (classInfo == null)
                {
                    return ResponseHandler.Failure<string>("Không tìm thấy lớp học");
                }

                var courseId = classInfo.CourseId;

                // Kiểm tra đăng ký hiện có
                var existingRegistration = await _unitOfWork.RegisterCourseRepository
                    .GetFirstOrDefaultAsync(rc => rc.CourseId == courseId
                                                && rc.UserId == studentId
                                                && (rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                                    || rc.StudentCourseStatus == StudentCourseStatusEnum.Pending
                                                   ));
                if (existingRegistration != null)
                {
                    return ResponseHandler.Failure<string>("Bạn đã đăng ký khóa học này");
                }

                // Kiểm tra xung đột lịch học
                var isConflict = await CheckScheduleConflict(studentId, classGuidId);
                if (isConflict)
                {
                    return ResponseHandler.Failure<string>("Lịch học của lớp này bị trùng với lịch học hiện tại của bạn");
                }

                // Tạo đăng ký mới
                var registerC = new RegisterCourse
                {
                    CourseId = courseId,
                    UserId = studentId,
                    StudentCourseStatus = StudentCourseStatusEnum.Enrolled,
                };
                await _unitOfWork.RegisterCourseRepository.AddAsync(registerC);
                await _unitOfWork.SaveChangeAsync();

                // Tạo URL thanh toán
                var paymentViewModel = new AgencyCoursePaymentViewModel
                {
                    CourseId = courseId.Value,
                    UserId = studentId,
                    AgencyId = student.AgencyId.Value,
                    RegisterCourseId = registerC.Id
                };
                var tempRegistration = new TempRegistrations
                {
                    UserId = studentId,
                    CourseId = courseId.Value,
                    ClassId = classGuidId,
                    CreationDate = DateTime.Now
                };
                await _unitOfWork.TempRegistrationsRepository.AddAsync(tempRegistration);
                var vnPayService = _serviceProvider.GetRequiredService<IVnPayService>();
                var paymentUrl = await vnPayService.CreatePaymentUrlForAgencyCourse(paymentViewModel);
                await _unitOfWork.SaveChangeAsync();
                response = ResponseHandler.Success(paymentUrl, "Đã tạo URL thanh toán thành công");
                return response;
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<string>(ex.Message);
            }
            return response;
        }

        // Cập nhật phương thức CheckScheduleConflict để sử dụng classId
        private async Task<bool> CheckScheduleConflict(string studentId, Guid newClassId)
        {
            var studentClasses = await _unitOfWork.ClassRoomRepository
                .GetAllAsync(cr => cr.UserId == studentId);

            var newClassSchedules = await _unitOfWork.ClassScheduleRepository
                .GetAllAsync1(cs => cs.ClassId == newClassId);

            foreach (var existingClass in studentClasses)
            {
                var existingSchedules = await _unitOfWork.ClassScheduleRepository
                    .GetAllAsync1(cs => cs.ClassId == existingClass.ClassId);

                foreach (var newSchedule in newClassSchedules)
                {
                    if (existingSchedules.Any(es =>
                        es.Date == newSchedule.Date &&
                        es.Slot.StartTime < newSchedule.Slot.EndTime &&
                        es.Slot.EndTime > newSchedule.Slot.StartTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private async Task<string> GetDateTimeFromRegisterCourseAsync(string userId, Guid courseId)
        {
            var registerCourses = await _unitOfWork.RegisterCourseRepository.GetAllAsync(rc =>
                rc.UserId == userId &&
                rc.CourseId == courseId &&
                (rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted || rc.StudentCourseStatus == StudentCourseStatusEnum.Pending));

            return registerCourses.FirstOrDefault()?.DateTime; // Lấy thời gian đầu tiên từ danh sách khóa học thỏa mãn điều kiện
        }
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }

}


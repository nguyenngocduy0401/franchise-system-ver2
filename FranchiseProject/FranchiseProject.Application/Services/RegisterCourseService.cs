using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
        public RegisterCourseService(IValidator<UpdateRegisterCourseViewModel> updateValidator, RoleManager<Role> roleManager,
            IEmailService emailService, IClaimsService claimsService,
            UserManager<User> userManager, IMapper mapper, 
            IUnitOfWork unitOfWork, IValidator<RegisterCourseViewModel> validator,
            ICurrentTime currentTime,IUserService userService)
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
        }


        public async Task<ApiResponse<bool>> RegisterCourseAsync(RegisterCourseViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);
                }
                var twentyFourHoursAgo = DateTime.Now.AddHours(-24);
                bool existsWithin24Hours = await _unitOfWork.RegisterCourseRepository.ExistsWithinLast24HoursAsync(model.StudentName,model.Email, model.PhoneNumber, model.CourseId);

                if (existsWithin24Hours)
                {
                    return ResponseHandler.Success<bool>(false, "Bạn đã đăng ký khóa học này trong vòng 24 giờ qua. Vui lòng thử lại sau.");
                }
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(Guid.Parse(model.CourseId));
                if (course == null) return ResponseHandler.Success<bool>(false, "Khóa học không khả dụng!");

                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(Guid.Parse(model.AgencyId));
                if (agency == null) return ResponseHandler.Success<bool>(false, "Trung tâm không khả dụng!");
               

                //tạo tài khoản 
                var newUser = new User
                {
                   FullName = model.StudentName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AgencyId = Guid.Parse(model.AgencyId),
                  //  StudentStatus = StudentStatusEnum.NotConsult,
                    Status = UserStatusEnum.active,
                    CreateAt = _currentTime.GetCurrentTime(),
                };
                var generate = await _userService.GenerateUserCredentials(newUser.FullName);
                newUser.UserName = generate.UserName;
                await _userManager.AddToRoleAsync(newUser, AppRole.Student);

                await _userManager.AddPasswordAsync(newUser, generate.Password);
                var result = await _userManager.UpdateAsync(newUser);

                if (!result.Succeeded)
                {
                    throw new Exception("Update User Account fail!");
                }
                //thêm vào lớp 
                var classScheduleEarliest = await _unitOfWork.ClassScheduleRepository.GetEarliestClassScheduleByClassIdAsync(model.ClassId.Value);
                var classScheduleLastest = await _unitOfWork.ClassScheduleRepository.GetLatestClassScheduleByClassIdAsync(model.ClassId.Value);
                var classRoom = new ClassRoom
                {
                    ClassId = model.ClassId,
                    UserId = newUser.Id,
                    FromDate = classScheduleEarliest.Date.HasValue ? DateOnly.FromDateTime(classScheduleEarliest.Date.Value) : null,
                    ToDate = classScheduleLastest.Date.HasValue ? DateOnly.FromDateTime(classScheduleLastest.Date.Value) : null, 

                };
                await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);
                //Thanh Toan

                var newRegisterCourse = new RegisterCourse
                {
                    UserId = newUser.Id,
                    CourseId = Guid.Parse(model.CourseId),
                    StudentCourseStatus = StudentCourseStatusEnum.NotConsult,
                   // ModificationDate=DateTime.Now
               
                };
                 await _unitOfWork.RegisterCourseRepository.AddAsync(newRegisterCourse);
                var emailMessage = EmailTemplate.SuccessRegisterCourseEmaill(model.Email, model.StudentName, course.Name, agency.Name);
                bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                if (!emailSent)
                {
                    return ResponseHandler.Success<bool>(false, "Lỗi khi gửi mail");
                }

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                return ResponseHandler.Success(true, "Đăng kí thành công!");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>(ex.Message);
            }
        }
      
        public async Task<ApiResponse<bool>> UpdateStatusStudentAsync( string studentId,string courseId, StudentCourseStatusEnum status)
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
                            registerCourse.StudentPaymentStatus=StudentPaymentStatusEnum.Pending_Payment;
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
                await  _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                await _unitOfWork.SaveChangeAsync();

                response = ResponseHandler.Success(true, "Cập nhật trạng thái học sinh thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }

            return response;
        }

        public async Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync(string id,string courseId)
        {
            var response = new ApiResponse<StudentRegisterViewModel>();

            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                var student = await _userManager.FindByIdAsync(id);
                if (student == null)
                {
                    return ResponseHandler.Success<StudentRegisterViewModel>(null,"Học sinh không tồn tại!");
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
                    return ResponseHandler.Success<StudentRegisterViewModel>(null,"Học sinh chưa đăng ký khóa học này!");
                }
                var rc = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc=> rc.UserId==id&&rc.CourseId==Guid.Parse(courseId));
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
                    UserId=student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    CourseCode = courseCodes.FirstOrDefault(),
                    StudentStatus = rc.StudentCourseStatus,
                    CourseId = registerCourse.CourseId,
                    DateTime = await GetDateTimeFromRegisterCourseAsync(id, registerCourse.CourseId.Value),
                    ConsultantName = consultantName,
                    CoursePrice = registerCourse.Course?.Price,
                    CreationDate=registerCourse.CreationDate,
                    ModificationDate = registerCourse.ModificationDate.ToString(),
                    PaymentStatus=registerCourse.StudentPaymentStatus,
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
                    return ResponseHandler.Success<Pagination<StudentRegisterViewModel>>(null,"User hoặc Agency không khả dụng!");
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
                    rc.User.AgencyId == userCurrent.AgencyId;
                var registerCourses = await _unitOfWork.RegisterCourseRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties:"User,Course",
                    pageIndex: filterStudentModel.PageIndex,
                    pageSize: filterStudentModel.PageSize
                );
              var registerCourseIds = registerCourses.Items.Select(rc => rc.Id).ToList();
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(
                    p => registerCourseIds.Contains((Guid)p.RegisterCourseId)
                );
                var paymentTotalByCourse = payments
                    .GroupBy(p => p.RegisterCourseId)
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

                var consultantIds = registerCourses.Items
                     .Where(item => item?.ConsultanId != null)
                     .Select(item => item.ConsultanId)
                     .Distinct()
                     .ToList();

                var consultants = await _userManager.Users
                    .Where(u => consultantIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);


                var studentRegisterViewModels = registerCourses.Items.OrderByDescending(item => item.CreationDate).Select(rc => new StudentRegisterViewModel
                {
                    Id = rc.Id,
                    UserId=rc.UserId,
                    CourseId = rc.CourseId,
                    FullName = rc.User?.FullName,
                    Email = rc.User?.Email,
                    PhoneNumber = rc.User?.PhoneNumber,
                    CourseCode = rc.Course?.Code,
                    CoursePrice = rc.Course?.Price,
                    CreationDate=rc.CreationDate,
                    ModificationDate = rc.ModificationDate.ToString(),
                    ConsultantName = rc.ConsultanId != null && consultants.TryGetValue(rc.ConsultanId, out var userName)
                        ? userName
                        : null,
                    StudentStatus = rc.StudentCourseStatus,
                    PaymentStatus = rc.StudentPaymentStatus,
                    DateTime = rc.DateTime,
                    StudentAmountPaid = paymentTotalByCourse.ContainsKey(rc.Id) ? paymentTotalByCourse[rc.Id] : 0,
                    PaymentDeadline =rc.PaymentDeadline,
                    
                }).ToList();
                var paginatedResult = new Pagination<StudentRegisterViewModel>
                {
                    Items = studentRegisterViewModels,
                    PageIndex = registerCourses.PageIndex,
                    PageSize = registerCourses.PageSize,
                    TotalItemsCount = registerCourses.TotalPagesCount
                };

                if (studentRegisterViewModels.Count==0)
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


        public async Task<ApiResponse<bool>> UpdateRegisterCourseDateTimeAsync(string id, UpdateRegisterCourseViewModel update)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var rc = await _unitOfWork.RegisterCourseRepository.GetExistByIdAsync(Guid.Parse(id));
                var userId = rc.UserId;
                var user =await  _userManager.FindByIdAsync(userId);
                user.FullName=update.StudentName;
                rc.DateTime=update.DateTime;
                rc.CourseId=Guid.Parse(update.CourseId);
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
        public async Task<ApiResponse<bool>> StudentExistRegisterCourse(string courseId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var courseGuidId = Guid.Parse(courseId);
                var studentId = _claimsService.GetCurrentUserId.ToString();

                var existingRegistration = await _unitOfWork.RegisterCourseRepository
             .GetFirstOrDefaultAsync(rc => rc.CourseId == courseGuidId
                                           && rc.UserId == studentId
                                           && (rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                               || rc.StudentCourseStatus == StudentCourseStatusEnum.Pending
                                              ));

                if (existingRegistration != null) 
                {
                     return ResponseHandler.Failure<bool>("Bạn đã đăng kí khóa học này ");
                }

                var registerC = new RegisterCourse { 
                CourseId= courseGuidId,
                UserId= studentId,
                StudentCourseStatus= StudentCourseStatusEnum.Pending,
                };
               await _unitOfWork.RegisterCourseRepository.AddAsync(registerC);
                await _unitOfWork.SaveChangeAsync();
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Đăng kí thất bại ");

                response = ResponseHandler.Success(true, "Đăng kí thành công");
                return response;
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }

            return response;
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


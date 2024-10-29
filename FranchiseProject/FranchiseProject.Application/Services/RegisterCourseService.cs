﻿using AutoMapper;
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
        public RegisterCourseService(IValidator<UpdateRegisterCourseViewModel> updateValidator, RoleManager<Role> roleManager,IEmailService emailService, IClaimsService claimsService,UserManager<User> userManager, IMapper mapper, IUnitOfWork unitOfWork, IValidator<RegisterCourseViewModel> validator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userManager = userManager;
            _claimsService = claimsService;
            _emailService = emailService;
            _roleManager = roleManager;
            _updateValidator = updateValidator;
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

              
                var nameParts = model.StudentName.Split(' ');
                var lastName = nameParts.LastOrDefault()?.ToLower(); 
                lastName = RemoveDiacritics(lastName);

                if (string.IsNullOrEmpty(lastName))
                {
                    return ResponseHandler.Failure<bool>("Tên người dùng không hợp lệ!");
                }
           
                string baseUserName = $"{lastName}lc";
                string finalUserName = baseUserName;
                int counter = 1;
                while (await _userManager.FindByNameAsync(finalUserName) != null)
                {
                    counter++;
                    finalUserName = $"{baseUserName}{counter}"; 
                }

      
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(Guid.Parse(model.CourseId));
                if (course == null) return ResponseHandler.Failure<bool>("Khóa học không khả dụng!");

                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(Guid.Parse(model.AgencyId));
                if (agency == null) return ResponseHandler.Failure<bool>("Trung tâm không khả dụng!");

                var newUser = new User
                {
                    UserName = finalUserName, 
                    FullName = model.StudentName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AgencyId = Guid.Parse(model.AgencyId),
                  //  StudentStatus = StudentStatusEnum.NotConsult,
                    Status = UserStatusEnum.active,
                    CreateAt = DateTime.Now,
                };

                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    return ResponseHandler.Failure<bool>("Không thể tạo tài khoản người dùng!");
                }

                var roleResult = await _userManager.AddToRoleAsync(newUser, AppRole.Student);
                if (!roleResult.Succeeded)
                {
                    return ResponseHandler.Failure<bool>("Không thể gán quyền cho người dùng!");
                }

                var newRegisterCourse = new RegisterCourse
                {
                    UserId = newUser.Id,
                    CourseId = Guid.Parse(model.CourseId),
                    StudentCourseStatus = StudentCourseStatusEnum.NotConsult
                   ,CreatDate=DateTime.Now,
                };
                 await _unitOfWork.RegisterCourseRepository.AddAsync(newRegisterCourse);
                var emailMessage = EmailTemplate.SuccessRegisterCourseEmaill(model.Email, model.StudentName, course.Name, agency.Name);
                bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                if (!emailSent)
                {
                    return ResponseHandler.Failure<bool>("Lỗi khi gửi mail");
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
                    return ResponseHandler.Failure<bool>("Học sinh không khả dụng!");
                }
                var courseGuidId = Guid.Parse(courseId);
                var registerCourse = await _unitOfWork.RegisterCourseRepository
                    .GetFirstOrDefaultAsync(rc => rc.UserId == studentId && rc.CourseId == courseGuidId);

                if (registerCourse == null)
                {
                    return ResponseHandler.Failure<bool>("Không tìm thấy bản ghi khóa học của học sinh!");
                }
                switch (status)
                {
                    case StudentCourseStatusEnum.Pending:
                        if (registerCourse.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        {
                            registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Pending;
                        }
                        else
                        {
                            return ResponseHandler.Failure<bool>("Học sinh không thể chuyển thành trạng thái Chờ!");
                        }
                        break;

                    case StudentCourseStatusEnum.Cancel:
                        registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Cancel;
                        break;
                    case StudentCourseStatusEnum.Waitlisted:
                        registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;
                        break;
                }

               
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
                    return ResponseHandler.Failure<StudentRegisterViewModel>("Học sinh không tồn tại!");
                }

                if (student.AgencyId != userCurrent.AgencyId)
                {
                    return ResponseHandler.Failure<StudentRegisterViewModel>("Học sinh không thuộc về agency của bạn!");
                }
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc =>
                    rc.UserId == id &&
                    rc.CourseId == Guid.Parse(courseId) 
                    );
                if (registerCourse == null)
                {
                    return ResponseHandler.Failure<StudentRegisterViewModel>("Học sinh chưa đăng ký khóa học này!");
                }
                var rc = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc=> rc.UserId==id&&rc.CourseId==Guid.Parse(courseId));
                var courseNames = await _unitOfWork.RegisterCourseRepository.GetCourseNamesByUserIdAsync(id);

                var studentViewModel = new StudentRegisterViewModel
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    CourseName = courseNames.FirstOrDefault(),
                    StudentStatus = rc.StudentCourseStatus,
                    CourseId = registerCourse.CourseId,
                    DateTime = await GetDateTimeFromRegisterCourseAsync(id, registerCourse.CourseId.Value),
                    CoursePrice = registerCourse.Course?.Price,
                    RegisterDate = registerCourse.CreatDate?.ToString()
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
                var studentRole = await _roleManager.FindByNameAsync(AppRole.Student);
                if (studentRole == null)
                {
                    return ResponseHandler.Failure<Pagination<StudentRegisterViewModel>>("Không tìm thấy vai trò sinh viên.");
                }
                var studentRoleId = studentRole.Id;
                var currentAgencyId = userCurrent.AgencyId;
                Expression<Func<User, bool>> filter = u =>
                     (u.AgencyId == currentAgencyId) &&
                     (u.UserRoles.Any(r => r.RoleId == studentRoleId)) &&
                     (u.StudentStatus != StudentStatusEnum.Enrolled) &&
                     (string.IsNullOrEmpty(filterStudentModel.CourseId) ||
                      u.RegisterCourses.Any(rc => rc.CourseId.ToString() == filterStudentModel.CourseId)) &&
                     (!filterStudentModel.Status.HasValue ||
                      u.RegisterCourses.Any(rc => rc.StudentCourseStatus == filterStudentModel.Status));

                var students = await _unitOfWork.UserRepository.GetFilterAsync(
                    filter: filter,
                 
                    pageIndex: filterStudentModel.PageIndex,
                    pageSize: filterStudentModel.PageSize,
                    includeProperties: "RegisterCourses.Course" 
                );
                var studentViewModels = students.Items
     .AsEnumerable()
     .Where(s => s.RegisterCourses.Any(rc =>
         rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
         rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted ||
         rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
         || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult))
     .OrderByDescending(s => s.RegisterCourses
         .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                      rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted ||
                      rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
         .Select(rc => rc.CreatDate)
         .FirstOrDefault())
     .Select(s => new StudentRegisterViewModel
     {
                    Id = s.Id,
                   
                    FullName = s.FullName,
                /*    StatusPayment=s.StudentPaymentStatus,*/
                    StudentStatus = s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                     || rc.StudentCourseStatus==StudentCourseStatusEnum.Cancel
                                     || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.StudentCourseStatus)
                        .FirstOrDefault(),
                    PhoneNumber = s.PhoneNumber,
                    Email = s.Email,
                    CourseId = s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.CourseId)
                        .FirstOrDefault(),
                    DateTime = s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.DateTime)
                        .FirstOrDefault(),
                    CourseName = s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.Course?.Name)
                        .FirstOrDefault(),
                    CoursePrice= s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.Course?.Price)
                        .FirstOrDefault(),
                    RegisterDate = s.RegisterCourses
                        .Where(rc => rc.StudentCourseStatus == StudentCourseStatusEnum.Pending ||
                                     rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.Cancel
                                      || rc.StudentCourseStatus == StudentCourseStatusEnum.NotConsult)
                        .Select(rc => rc.CreatDate)
                        .FirstOrDefault().ToString(),

                }).ToList();

                var paginatedResult = new Pagination<StudentRegisterViewModel>
                {
                    Items = studentViewModels,
                    TotalItemsCount = studentViewModels.Count,
                    PageIndex = students.PageIndex,
                    PageSize = students.PageSize
                };

                if (!studentViewModels.Any())
                    return ResponseHandler.Success(paginatedResult, "Không tìm thấy sinh viên phù hợp!");

                response = ResponseHandler.Success(paginatedResult, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<StudentRegisterViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateRegisterCourseDateTimeAsync(string userId, string courseId, UpdateRegisterCourseViewModel update)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(
                    rc => rc.UserId == userId && rc.CourseId ==  Guid.Parse(courseId)&& rc.StudentCourseStatus==StudentCourseStatusEnum.Pending);
                if (registerCourse == null)
                {
                    return ResponseHandler.Failure<bool>("Không tìm thấy khóa học đăng ký phù hợp.");
                }
                
                if (update.CourseId != null) 
                {
                    var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(Guid.Parse(update.CourseId));
                    if (course == null) { return ResponseHandler.Failure<bool>("Không tìm thấy khóa học phù hợp."); }
                    _unitOfWork.RegisterCourseRepository.Delete(registerCourse);
                }
                
                if (update.DateTime !="")
                {
                    var dateTimePattern = @"^(Thứ [2-7]|Chủ Nhật)(,\s*(Thứ [2-7]|Chủ Nhật))*;\s*[0-2][0-9]:[0-5][0-9]$";
                    if (!Regex.IsMatch(update.DateTime, dateTimePattern, RegexOptions.IgnoreCase))
                    {
                        return ResponseHandler.Failure<bool>("Định dạng không hợp lệ! Định dạng đúng: Thứ X, Thứ Y;HH:mm.");
                    }

                    var dateTimeParts = update.DateTime.Split(';');
                    var daysOfWeek = dateTimeParts[0].Trim().Split(',');
                    var time = TimeSpan.Parse(dateTimeParts[1].Trim());


                    var validDaysOfWeek = new HashSet<string>
        {
            "thứ 2", "thứ 3", "thứ 4", "thứ 5", "thứ 6", "thứ 7", "chủ nhật"
        };


                    foreach (var day in daysOfWeek)
                    {
                        var trimmedDay = day.Trim().ToLower(); 
                        if (!validDaysOfWeek.Contains(trimmedDay))
                        {
                            return ResponseHandler.Failure<bool>("Ngày trong tuần không hợp lệ. Chỉ chấp nhận Thứ 2 đến Thứ 7 hoặc Chủ Nhật.");
                        }
                    }
                    var slot = await _unitOfWork.SlotRepository.GetFirstOrDefaultAsync(s => s.StartTime == time);
                    if (slot == null)
                    {
                        return ResponseHandler.Failure<bool>("Không có Slot nào khớp với thời gian đã nhập!");
                    }
                }
                if (update.CourseId != null)
                {
                    var newRegisterCourse = new RegisterCourse
                    {
                        UserId = userId,
                        CourseId = Guid.Parse(update.CourseId),
                        DateTime = update.DateTime,
                        StudentCourseStatus = StudentCourseStatusEnum.Pending
                    };
                    await _unitOfWork.RegisterCourseRepository.AddAsync(newRegisterCourse);
                }
                else
                {
                    var registerCourseExist = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(
                   rc => rc.UserId == userId && rc.CourseId == Guid.Parse(courseId) && rc.StudentCourseStatus == StudentCourseStatusEnum.Pending);
                    registerCourseExist.DateTime = update.DateTime;
                    await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourseExist);
                }
                if (update.StudentName != null)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    var nameParts = update.StudentName.Split(' ');
                    var lastName = nameParts.LastOrDefault()?.ToLower();
                    lastName = RemoveDiacritics(lastName);

                    if (string.IsNullOrEmpty(lastName))
                    {
                        return ResponseHandler.Failure<bool>("Tên người dùng không hợp lệ!");
                    }

                    string baseUserName = $"{lastName}lc";
                    string finalUserName = baseUserName;
                    int counter = 1;
                    user.UserName=finalUserName;
                    user.FullName = update.StudentName;
                }
              

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    return ResponseHandler.Failure<bool>("Cập nhật thất bại!");
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
                                               ||rc.StudentCourseStatus == StudentCourseStatusEnum.Studied));

                if (existingRegistration != null) 
                {
                     return ResponseHandler.Failure<bool>("Bạn đã đăng kí khóa học này ");
                }

                var registerC = new RegisterCourse { 
                CourseId= courseGuidId,
                UserId= studentId,
                CreatDate= DateTime.Now,
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


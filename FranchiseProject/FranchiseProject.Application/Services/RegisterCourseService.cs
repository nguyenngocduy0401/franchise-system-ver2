using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Handler.EmailTemplateHandler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.Pkcs;
using System.Text;
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
        public RegisterCourseService(IClaimsService claimsService,UserManager<User> userManager, IMapper mapper, IUnitOfWork unitOfWork, IValidator<RegisterCourseViewModel> validator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _userManager = userManager;
            _claimsService = claimsService;
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
                var course=await _unitOfWork.CourseRepository.GetExistByIdAsync(Guid.Parse(model.CourseId));
                if (course == null) {return ResponseHandler.Failure<bool>("Khóa học không khả dụng!"); }
                var agency= await  _unitOfWork.AgencyRepository.GetExistByIdAsync(Guid.Parse(model.AgencyId));
                if (agency == null) { return ResponseHandler.Failure<bool>("Trung tâm không khả dụng!"); }
                var newUser = new User
                {
                    UserName = model.StudentName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    AgencyId=Guid.Parse(model.AgencyId),
                    StudentStatus=StudentStatusEnum.NotConsult,
                    Status=UserStatusEnum.active,   
                    
                    
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
                    StudentCourseStatus=StudentCourseStatusEnum.NotStudied
                };
                var emailMessage = EmailTemplateHandler.SuccessRegisterCourseEmaill(model.Email, model.StudentName, course.Name, agency.Name);
                bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                if (!emailSent)
                {
                    response = ResponseHandler.Failure<bool>("Lỗi khi gửi mail");
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Đăng kí thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateStatusStudentAsync(StudentStatusEnum studentStatus, string studentId)
        {
          var response= new ApiResponse<bool>();
            try
            {
                var student= await _unitOfWork.UserRepository.GetStudentByIdAsync(studentId);
                if (student == null) { return ResponseHandler.Failure<bool>("Học sinh không khả dụng!"); }
                switch (studentStatus) 
                {
                    case StudentStatusEnum.Pending:
                        if(student.StudentStatus == StudentStatusEnum.NotConsult)
                        {
                            student.StudentStatus = StudentStatusEnum.Pending;
                        }else
                        {
                            return ResponseHandler.Failure<bool>("Học sinh không thể chuyển thành trạng thái Chờ!");
                        }
                        break;
                    case StudentStatusEnum.Waitlisted:
                        //Nếu có lỗi hệ thống gì đó khi mà thanh toán thành công mà không chuyển qua wailisted
                        if (student.StudentPaymentStatus == StudentPaymentStatusEnum.Completed)
                        {
                            student.StudentStatus = StudentStatusEnum.Waitlisted;
                        }
                        break;
                    
                }
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync(string id)
        {
            var response = new ApiResponse<StudentRegisterViewModel>();
            try
            {
                var userCurrentId  =  _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var student = await _unitOfWork.UserRepository.GetStudentByIdAsync(id);
                if (student == null) throw new Exception("Student does not exist!");

                //cần check Student thuộc agency 
                var agency = _unitOfWork.AgencyRepository.GetExistByIdAsync(userCurrent.AgencyId.Value);
                if (student.AgencyId != userCurrent.AgencyId)
                {
                    throw new Exception("Student does not belong to your agency!");
                }


                var courseNames = await _unitOfWork.RegisterCourseRepository
                    .GetCourseNamesByUserIdAsync(id); 
                var studentViewModel = _mapper.Map<StudentRegisterViewModel>(student);
                studentViewModel.CourseName = courseNames;
                response = ResponseHandler.Success(studentViewModel, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<StudentRegisterViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<StudentViewModel>>> FilterStudentAsync(FilterStudentViewModel filterStudentModel)
        {
            var response = new ApiResponse<Pagination<StudentViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                // Kiểm tra agency của người dùng hiện tại
                var currentAgencyId = userCurrent.AgencyId;
                Expression<Func<User, bool>> filter = u =>
                 (u.AgencyId == currentAgencyId) &&
                    (!filterStudentModel.StatusPayment.HasValue || u.StudentPaymentStatus == filterStudentModel.StatusPayment) &&
                    (!filterStudentModel.Status.HasValue || u.StudentStatus == filterStudentModel.Status)&&
                    (string.IsNullOrEmpty(filterStudentModel.CourseId) ||
                    u.RegisterCourses.Any(rc => rc.CourseId.ToString() == filterStudentModel.CourseId));
                var students = await _unitOfWork.UserRepository.GetFilterAsync(
                    filter: filter,
                    
                    pageIndex: filterStudentModel.PageIndex,
                    pageSize: filterStudentModel.PageSize,
                    includeProperties:"Course"
                );

                var studentViewModels = _mapper.Map<Pagination<StudentViewModel>>(students);

                if (studentViewModels.Items.IsNullOrEmpty())
                    return ResponseHandler.Success(studentViewModels, "Không tìm thấy sinh viên phù hợp!");

                response = ResponseHandler.Success(studentViewModels, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<StudentViewModel>>(ex.Message);
            }
            return response;
        }

    }
}

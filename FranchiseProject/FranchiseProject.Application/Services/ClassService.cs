using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ClassService : IClassService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateClassViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public ClassService(IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateClassViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                //Check List Học sinh có có đủ điều kiện không :Student đó phải có Status là waitlisted 
                if (model.StudentId == null || model.StudentId.Count == 0)
                {
                    return ResponseHandler.Failure<bool>("Danh sách học sinh không hợp lệ!");
                }

                // Kiểm tra trạng thái của từng học sinh
                var students = await _unitOfWork.UserRepository.GetAllAsync(u => model.StudentId.Contains(u.Id));
                var waitlistedStudents = await _unitOfWork.ClassRoomRepository.GetWaitlistedStudentsAsync(model.StudentId);
                if (waitlistedStudents.Count == 0)
                {
                    // Lấy danh sách học sinh không hợp lệ
                    var invalidStudents = await _unitOfWork.ClassRoomRepository.GetInvalidStudentsAsync(model.StudentId);
                    var invalidStudentNames = string.Join(", ", invalidStudents);
                    return ResponseHandler.Failure<bool>($"Không có học sinh nào có trạng thái 'waitlisted'! Các học sinh không hợp lệ: {invalidStudentNames}");
                }
                //Tạo Các Chuyển Đổi trạng thái các Student Thành Erolled 
                foreach (var student in waitlistedStudents)
                {
                    student.StudentStatus = StudentStatusEnum.Enrolled;
                    var updateResult = await _userManager.UpdateAsync(student);

                }
                //Chuyển Tất Cả bảng Record UserId gắn Với Courseid RegisterCourseStatus thành CurrentStudies   
                var registerCourses = await _unitOfWork.RegisterCourseRepository.GetAllAsync(rc => model.StudentId.Contains(rc.UserId) && rc.CourseId == Guid.Parse(model.CourseId));
                foreach (var registerCourse in registerCourses)
                {
                    registerCourse.StudentCourseStatus = StudentCourseStatusEnum.CurrentlyStudying;
                    await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                }
                //Tạo Class
                var newClass = new Class
                {
                    Name = model.Name,
                    Capacity = model.Capacity,
                    CourseId = Guid.Parse(model.CourseId),
                    Status = ClassStatusEnum.Inactive
                };
                await _unitOfWork.ClassRepository.AddAsync(newClass);
                foreach (var student in waitlistedStudents)
                {

                    var classRoom = new ClassRoom
                    {
                        ClassId= newClass.Id,
                        UserId= student.Id,
                    };
                    await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo lớp  học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

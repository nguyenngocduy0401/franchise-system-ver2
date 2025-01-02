﻿using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using iText.Kernel.Geom;
using MailKit.Search;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ClassService : IClassService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateClassViewModel> _validator;
        private readonly IValidator<UpdateClassViewModel> _validatorUpdate;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public ClassService(IValidator<UpdateClassViewModel> validatorUpdate, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateClassViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
            _validatorUpdate = validatorUpdate;
        }
        #endregion
        public async Task<ApiResponse<Guid?>> CreateClassAsync(CreateClassViewModel model)
        {
            var response = new ApiResponse<Guid?>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var courseId = Guid.Parse(model.CourseId);
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<Guid?>(validationResult);
                }
                if (!string.IsNullOrEmpty(model.InstructorId))
                {
                    var instructor = await _userManager.FindByIdAsync(model.InstructorId);
                    if (instructor == null || !await _userManager.IsInRoleAsync(instructor, "Instructor"))
                    {
                        return ResponseHandler.Failure<Guid?>("Người hướng dẫn không hợp lệ hoặc không có vai trò là 'Instructor'!");
                    }
                }
                //Check List Học sinh có có đủ điều kiện không ==>Student đó phải có Status là waitlisted 

                var classWithSameName = await _unitOfWork.ClassRepository.GetFirstOrDefaultAsync(c => c.Name == model.Name && !c.IsDeleted);
                if (classWithSameName != null)
                {
                    return ResponseHandler.Success<Guid?>(null, $"Tên lớp '{model.Name}' đã tồn tại!");
                }
                if (model.StudentId.Count > model.Capacity)
                {
                    return ResponseHandler.Success<Guid?>(null, $"Số lượng học sinh vượt quá sức chứa lớp học! Sức chứa tối đa: {model.Capacity}.");
                }
                var invalidCourseRegistrations = await _unitOfWork.RegisterCourseRepository.GetAllAsync(rc =>
                    model.StudentId.Contains(rc.UserId) && rc.CourseId != Guid.Parse(model.CourseId) && rc.StudentCourseStatus != StudentCourseStatusEnum.Waitlisted);

                var newClass = new Class
                {
                    Name = model.Name,
                    Capacity = model.Capacity,
                    CourseId = Guid.Parse(model.CourseId),
                    Status = ClassStatusEnum.Inactive,
                    AgencyId = userCurrent.AgencyId
                  ,
                    CurrentEnrollment = model.StudentId.Count
                };

                var quizAndAssignmentModel = await CreateQuizAndAssignmentForClass(courseId);

                newClass.Quizzes = quizAndAssignmentModel.Quizzes;
                newClass.Assignments = quizAndAssignmentModel.Assignments;


                await _unitOfWork.ClassRepository.AddAsync(newClass);
                // Kiểm tra trạng thái của từng học sinh
                // var students = await _unitOfWork.UserRepository.GetAllAsync(u => model.StudentId.Contains(u.Id));

                if (model.StudentId != null && model.StudentId.Any())
                {
                    var waitlistedStudents = await _unitOfWork.ClassRoomRepository.CheckWaitlistedStatusForStudentsAsync(model.StudentId, courseId);
                    foreach (var student in model.StudentId)
                    {
                        var check2 = await _unitOfWork.RegisterCourseRepository.FindRegisterCourseByUserId(student, Guid.Parse(model.CourseId));
                        if (check2 == null) { return ResponseHandler.Success<Guid?>(null, "Danh sánh học sinh chưa hợp lệ !"); }
                        check2.StudentCourseStatus = StudentCourseStatusEnum.Enrolled;
                        await _unitOfWork.RegisterCourseRepository.UpdateAsync(check2);
                        var newClassRoom = new ClassRoom
                        {
                            UserId = student,
                            ClassId = newClass.Id,

                        };

                        await _unitOfWork.ClassRoomRepository.AddAsync(newClassRoom);
                    }
                }
                if (!string.IsNullOrEmpty(model.InstructorId))
                {

                    var classRoom1 = new ClassRoom
                    {
                        ClassId = newClass.Id,
                        UserId = model.InstructorId,
                    };

                    await _unitOfWork.ClassRoomRepository.AddAsync(classRoom1);
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success((Guid?)newClass.Id, "Tạo lớp  học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Guid?>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateClassAsync(string id, UpdateClassViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validatorUpdate.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);
                }
                var classId = Guid.Parse(id);

                var existingClass = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                if (existingClass == null)
                {
                    return ResponseHandler.Success(false, "Không tìm thấy lớp học.");
                }
                var isNameExist = await _unitOfWork.ClassRepository.AnyAsync(c => c.Name == model.Name && !c.IsDeleted && c.Id != classId);
                if (isNameExist)
                {
                    return ResponseHandler.Success<bool>(false, "Tên lớp học đã tồn tại.");
                }
                if (model.Capacity < existingClass.CurrentEnrollment)
                {
                    return ResponseHandler.Success<bool>(false, "Số lượng không thể nhỏ hơn số học sinh đang trong lớp ");
                }
                existingClass.Name = model.Name;
                existingClass.Capacity = model.Capacity;
                if (!string.IsNullOrEmpty(model.InstructorId))
                {
                    var rc = await _unitOfWork.ClassRoomRepository.GetClassRoomsByClassIdAndInstructorRoleAsync(classId);
                    if (rc == null)
                    {
                        var classRoom = new ClassRoom
                        {
                            ClassId = classId,
                            UserId = model.InstructorId,
                        };
                        await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);
                    }
                    else
                    {
                        await _unitOfWork.ClassRoomRepository.DeleteAsync(rc);
                        var classRoom1 = new ClassRoom
                        {
                            ClassId = classId,
                            UserId = model.InstructorId,
                        };
                        await _unitOfWork.ClassRoomRepository.AddAsync(classRoom1);
                    }

                }
                _unitOfWork.ClassRepository.Update(existingClass);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                if (!isSuccess) throw new Exception("Cập nhật lớp học thất bại.");

                response = ResponseHandler.Success(true, "Cập nhật lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync(FilterClassViewModel filterClassModel)
        {
            var response = new ApiResponse<Pagination<ClassViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<Pagination<ClassViewModel>>(null, "User hoặc Agency không khả dụng!");
                }

                var query = _unitOfWork.ClassRepository.GetQueryable()
                    .Include(c => c.Course)
                    .Include(c => c.ClassSchedules)
                    .Where(c => c.AgencyId == userCurrent.AgencyId);

                if (!string.IsNullOrEmpty(filterClassModel.Name))
                {
                    query = query.Where(c => c.Name.Contains(filterClassModel.Name));
                }

                if (filterClassModel.Status.HasValue)
                {
                    query = query.Where(c => c.Status == filterClassModel.Status);
                }

                if (!string.IsNullOrEmpty(filterClassModel.CourseId))
                {
                    var courseId = Guid.Parse(filterClassModel.CourseId);
                    query = query.Where(c => c.CourseId == courseId);
                }

                if (!string.IsNullOrEmpty(filterClassModel.DayOfWeek))
                {
                    query = query.Where(c => c.DayOfWeek.Contains(filterClassModel.DayOfWeek));
                }

                if (filterClassModel.SlotId.HasValue)
                {
                    query = query.Where(c => c.ClassSchedules.Any(cs => cs.SlotId == filterClassModel.SlotId));
                }

                var totalCount = await query.CountAsync();

                var classes = await query
                    .Skip((filterClassModel.PageIndex - 1) * filterClassModel.PageSize)
                    .Take(filterClassModel.PageSize)
                    .ToListAsync();

                var classViewModels = new List<ClassViewModel>();

                foreach (var c in classes)
                {
                    var classRoomInstructors = await _unitOfWork.ClassRoomRepository
                        .GetAllAsync(cr => cr.ClassId == c.Id);

                    string instructorName = null;

                    foreach (var cr in classRoomInstructors)
                    {
                        var user = await _userManager.FindByIdAsync(cr.UserId);
                        if (user != null)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
                            if (roles.Contains(AppRole.Instructor))
                            {
                                instructorName = user.UserName;
                                break;
                            }
                        }
                    }

                    classViewModels.Add(new ClassViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Capacity = c.Capacity,
                        CurrentEnrollment = c.CurrentEnrollment,
                        DayOfWeek = c.DayOfWeek,
                        CourseName = c.Course.Name,
                        Status = c.Status.Value,
                        CourseId = c.CourseId,
                        InstructorName = instructorName
                    });
                }

                var paginatedResult = new Pagination<ClassViewModel>
                {
                    Items = classViewModels,
                    TotalItemsCount = totalCount,
                    PageIndex = filterClassModel.PageIndex,
                    PageSize = filterClassModel.PageSize
                };

                response = ResponseHandler.Success(paginatedResult, "Lọc lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<ClassViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<ClassViewModel>> GetClassByIdAsync(string id)
        {
            var response = new ApiResponse<ClassViewModel>();
            try
            {
                var classId = Guid.Parse(id);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                if (classEntity == null)
                {
                    return ResponseHandler.Success<ClassViewModel>(null, "Không tìm thấy lớp học!");
                }
                var classViewModel = _mapper.Map<ClassViewModel>(classEntity);
                response = ResponseHandler.Success(classViewModel, "Lấy thông tin lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<ClassViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateClassStatusAsync(ClassStatusEnum status, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {

                var classId = Guid.Parse(id);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                if (classEntity == null)
                {
                    return ResponseHandler.Success<bool>(false, "Không tìm thấy lớp học!");
                }
                classEntity.Status = status;
                _unitOfWork.ClassRepository.Update(classEntity);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    throw new Exception("Cập nhật trạng thái thất bại!");
                }
                response = ResponseHandler.Success(true, "Cập nhật trạng thái lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<ClassStudentViewModel>> GetClassDetailAsync(string id)
        {
            var response = new ApiResponse<ClassStudentViewModel>();
            try
            {
                var classId = Guid.Parse(id);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                if (classEntity == null)
                {
                    return ResponseHandler.Success<ClassStudentViewModel>(null, "Không tìm thấy lớp học!");
                }

                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classEntity.Id);
                var courseEntity = classEntity.CourseId.HasValue
                    ? await _unitOfWork.CourseRepository.GetByIdAsync(classEntity.CourseId.Value)
                    : null;

                var studentInfo = new List<StudentClassViewModel>();

                var classSchedules = await _unitOfWork.SlotRepository
                     .GetAllSlotAsync(cs => cs.ClassId == classEntity.Id);

                var slotViewModels = new List<SlotViewModel>();
                string instructorName = string.Empty;
                var studentIdsAdded = new HashSet<string>();
                string insId = string.Empty;
                foreach (var cr in classRooms)
                {
                    var user = await _userManager.FindByIdAsync(cr.UserId);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        if (roles.Contains(AppRole.Student) && !studentIdsAdded.Contains(cr.UserId))
                        {
                            studentInfo.Add(new StudentClassViewModel
                            {

                                UserId = cr.UserId,
                                UserName = user.UserName,
                                StudentName = user.FullName,
                                DateOfBirth = user.DateOfBirth,
                                URLImage = user.URLImage

                            });

                            studentIdsAdded.Add(cr.UserId);
                        }

                        if (roles.Contains(AppRole.Instructor))
                        {
                            instructorName = user.UserName;
                            insId = user.Id;
                        }
                    }
                }

                foreach (var schedule in classSchedules)
                {
                    if (schedule.SlotId.HasValue)
                    {
                        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(schedule.SlotId.Value);
                        if (slot != null)
                        {
                            slotViewModels.Add(new SlotViewModel
                            {
                                Id = slot.Id,
                                Name = slot.Name,
                                StartTime = slot.StartTime,
                                EndTime = slot.EndTime
                            });
                        }
                    }
                }
                var classDetail = new ClassStudentViewModel
                {
                    ClassId = classEntity.Id,
                    ClassName = classEntity.Name,
                    Capacity = classEntity.Capacity,
                    CurrentEnrollment = classEntity.CurrentEnrollment,
                    DayOfWeek = classEntity.DayOfWeek,
                    CourseId = classEntity.CourseId,
                    CourseCode = courseEntity?.Code,
                    CourseName = courseEntity?.Name,
                    InstructorId = insId,
                    InstructorName = instructorName,
                    StudentInfo = studentInfo,
                    SlotViewModels = slotViewModels.FirstOrDefault()
                };



                response = ResponseHandler.Success(classDetail, "Lấy thông tin lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<ClassStudentViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> AddStudentAsync(string classId, AddStudentViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classE = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(classId));
                var courseId = classE.CourseId.Value;
                var students = await _unitOfWork.UserRepository.GetAllAsync(u => model.StudentId.Contains(u.Id));
                var waitlistedStudents = await _unitOfWork.ClassRoomRepository.CheckWaitlistedStatusForStudentsAsync(model.StudentId, courseId);
                if (classE.Capacity == classE.CurrentEnrollment) { return ResponseHandler.Success(false, "Lớp đã đã đủ học sinh!"); }
                if (!waitlistedStudents.Any())
                {
                    return ResponseHandler.Success(false, "Không có sinh viên nào có trạng thái chờ lớp  để thêm vào lớp học.");
                }
                var classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == Guid.Parse(classId));
                var sortedSchedules = classSchedules.OrderBy(cs => cs.Date).ToList();

                var currentDate = DateTime.Now.Date;
                var totalSchedules = sortedSchedules.Count;
                var passedSchedules = sortedSchedules.Count(s => s.Date < currentDate);

                if ((double)passedSchedules / totalSchedules > 0.1)
                {
                    return ResponseHandler.Success(false, "Không thể thêm học sinh vì lớp đã qua hơn 10% số buổi học.");
                }

                foreach (var studentId in waitlistedStudents.Keys)
                {
                    if (waitlistedStudents[studentId])
                    {
                        var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc =>
                            rc.UserId == studentId && rc.CourseId == courseId);

                        if (registerCourse != null)
                        {
                            registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Enrolled;


                            await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                            var classRoom = new ClassRoom { ClassId = Guid.Parse(classId), UserId = studentId };
                            await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);
                            classE.CurrentEnrollment = classE.CurrentEnrollment + 1;
                        }
                    }
                    else
                    {
                        return ResponseHandler.Success<bool>(false, "thêm học sinh không thành công!");
                    }
                }
                await _unitOfWork.SaveChangeAsync();
                response = ResponseHandler.Success(true, "Thêm sinh viên vào lớp học thành công !");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }

            return response;
        }
        public async Task<ApiResponse<bool>> RemoveStudentAsync(string studentId, string classId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(Guid.Parse(classId));
                if (classEntity == null)
                {
                    return ResponseHandler.Success<bool>(false, "Lớp học không tồn tại!");
                }
                var classE = await _unitOfWork.ClassRepository.GetExistByIdAsync(Guid.Parse(classId));
                var courseId = classE.CourseId;
                var classRoom = await _unitOfWork.ClassRoomRepository.GetFirstOrDefaultAsync(cr => cr.UserId == studentId && cr.ClassId == Guid.Parse(classId));
                var rc = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(cr => cr.UserId == studentId && cr.CourseId == courseId && cr.StudentCourseStatus == StudentCourseStatusEnum.Enrolled);
                if (classRoom == null)
                {
                    return ResponseHandler.Success<bool>(false, "Học sinh không có trong lớp học này!");
                }

                var student = await _userManager.FindByIdAsync(studentId);
                if (student == null)
                {
                    return ResponseHandler.Success<bool>(false, "Học sinh không tồn tại!");
                }
                rc.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;
                var updateStudentResult = _unitOfWork.RegisterCourseRepository.UpdateAsync(rc);
                if (!updateStudentResult.IsCompleted)
                {
                    throw new Exception("Cập nhật trạng thái học sinh thất bại!");
                }
                await _unitOfWork.ClassRoomRepository.DeleteAsync(classRoom);
                classEntity.CurrentEnrollment -= 1;
                _unitOfWork.ClassRepository.Update(classEntity);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Xóa học sinh thất bại!");

                response = ResponseHandler.Success(true, "Xóa học sinh thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteClassAsync(string classId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classID = Guid.Parse(classId);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classID);
                if (classEntity == null)
                {
                    return ResponseHandler.Success<bool>(false, "Lớp không tồn tại hoặc đã bị xóa!");
                }


                classEntity.IsDeleted = true;
                _unitOfWork.ClassRepository.Update(classEntity);
                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classID);
                var studentIds = await _unitOfWork.ClassRoomRepository.GetUserIdsByClassIdAsync(classID);
                foreach (var classRoom in classRooms)
                {

                    var classE = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(classId));
                    var courseId = classE.CourseId.Value;
                    var students = await _unitOfWork.UserRepository.GetAllAsync(u => classRoom.UserId.Contains(u.Id));
                    var waitlistedStudents = await _unitOfWork.ClassRoomRepository.CheckEnrollStatusForStudentsAsync(studentIds, courseId);

                    foreach (var studentId in waitlistedStudents.Keys)
                    {
                        if (waitlistedStudents[studentId])
                        {
                            var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc =>
                                rc.UserId == studentId && rc.CourseId == courseId);

                            if (registerCourse != null)
                            {
                                registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;


                                await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                            }
                        }

                    }
                    await _unitOfWork.SaveChangeAsync();
                }
                foreach (var classRoom in classRooms)
                {
                    await _unitOfWork.ClassRoomRepository.DeleteAsync(classRoom);
                }
                response = ResponseHandler.Success(true, "Lớp đã được xóa thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<ClassScheduleViewModel>>> GetClassSchedulesByClassIdAsync(string classId, DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<List<ClassScheduleViewModel>>();
            try
            {
                var classGuidId = Guid.Parse(classId);

                List<ClassSchedule> classSchedules;

                if (startDate == default(DateTime) && endDate == default(DateTime))

                {
                    classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == classGuidId);
                }
                else
                {
                    classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs =>
                        cs.ClassId == classGuidId && cs.Date.Value.Date >= startDate.Date && cs.Date.Value.Date <= endDate.Date);
                }

                var classE = await _unitOfWork.ClassRepository.GetByIdAsync(classGuidId);

                var classScheduleViewModels = new List<ClassScheduleViewModel>();
                if (classSchedules == null || !classSchedules.Any())
                {
                    return ResponseHandler.Failure<List<ClassScheduleViewModel>>("Không tìm thấy lịch học cho lớp này trong khoảng thời gian đã cho!");
                }

                foreach (var schedule in classSchedules)
                {
                    var slot = await _unitOfWork.SlotRepository.GetByIdAsync(schedule.SlotId.Value);

                    classScheduleViewModels.Add(new ClassScheduleViewModel
                    {
                        Id = schedule.Id.ToString(),
                        Room = schedule.Room,
                        ClassId = classE.Id,
                        ClassName = classE.Name,
                        SlotName = slot?.Name,
                        Date = schedule.Date.ToString(),
                        StartTime = slot?.StartTime.ToString(),
                        EndTime = slot?.EndTime.ToString()
                    });
                }

                response = ResponseHandler.Success(classScheduleViewModels, "Lấy danh sách lịch học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<ClassScheduleViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<InstructorViewModel>>> GetInstructorsByAgencyAsync()
        {
            var response = new ApiResponse<List<InstructorViewModel>>();
            try
            {
                var userid = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userid);

                var instructors = await _unitOfWork.UserRepository.GetInstructorsByAgencyIdAsync(user.AgencyId.Value);
                var instructorViewModels = instructors.Select(i => new InstructorViewModel
                {
                    Id = i.Id,
                    UserName = i.UserName,
                    FullName = i.FullName
                }).ToList();

                response = ResponseHandler.Success(instructorViewModels, "Lấy danh sách Instructor thành công.");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<InstructorViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<StudentScheduleViewModel>>> GetStudentSchedulesAsync(DateTime startTime, DateTime endTime)
        {
            var response = new ApiResponse<List<StudentScheduleViewModel>>();
            try
            {
                var studentId = _claimsService.GetCurrentUserId.ToString();

                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.UserId == studentId);

                if (!classRooms.Any())
                {
                    return ResponseHandler.Success<List<StudentScheduleViewModel>>(null, "Không tìm thấy lịch học cho sinh viên này.");
                }

                var classIds = classRooms.Select(cr => cr.ClassId).Distinct().ToList();
                var activeClassIds = new List<Guid>();

                foreach (var classId in classIds)
                {
                    var check1 = await _unitOfWork.ClassRepository.GetByIdAsync(classId.Value);
                    if (check1 != null)
                    {
                        activeClassIds.Add(classId.Value);
                    }
                }


                var schedules = startTime != DateTime.MinValue && endTime != DateTime.MinValue
                         ? await _unitOfWork.ClassScheduleRepository.GetClassSchedulesByClassIdsAsync(
                             activeClassIds,
                             cs => cs.Date.HasValue &&
                                   cs.Date.Value.Date >= startTime.Date &&
                                   cs.Date.Value.Date <= endTime.Date
                           )
                         : await _unitOfWork.ClassScheduleRepository.GetClassSchedulesByClassIdsAsync(
                             activeClassIds,
                             cs => cs.Date.HasValue
                           );

                var scheduleViewModels = new List<StudentScheduleViewModel>();


                foreach (var schedule in schedules)
                {
                    var slot = schedule.SlotId.HasValue
                        ? await _unitOfWork.SlotRepository.GetByIdAsync(schedule.SlotId.Value)
                        : null;
                    var attendance = await _unitOfWork.AttendanceRepository.GetFirstOrDefaultAsync(a =>
                                  a.ClassScheduleId == schedule.Id && a.UserId == studentId);

                    var instructor = await _unitOfWork.UserRepository.GetInstructorsByClassIdAsync(schedule.ClassId.Value);

                    string insId = instructor.Id;
                    string instructorName = instructor.FullName;

                    scheduleViewModels.Add(new StudentScheduleViewModel
                    {
                        ScheduleId = schedule.Id,
                        ClassId = schedule.ClassId.Value,
                        SlotId = schedule.SlotId ?? Guid.Empty,
                        Room = schedule.Room,
                        ClassName = schedule.Class.Name,
                        SlotName = slot?.Name,
                        Date = schedule.Date,
                        StartTime = slot?.StartTime ?? TimeSpan.Zero,
                        EndTime = slot?.EndTime ?? TimeSpan.Zero,
                        AttendanceStatus = attendance?.Status,
                        Status = schedule.Status,
                        Url = schedule.Url,
                        TeacherId = insId,
                        TeacherName = instructorName
                    });
                }

                response = ResponseHandler.Success(scheduleViewModels, "Lấy danh sách lịch học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<StudentScheduleViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<ClassViewModel>>> GetAllClassByCourseId(string courseId)
        {
            var response = new ApiResponse<List<ClassViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                var agencyId = userCurrent?.AgencyId;
                if (!Guid.TryParse(courseId, out Guid parsedCourseId))
                {
                    return ResponseHandler.Success<List<ClassViewModel>>(null, "CourseId không hợp lệ.");
                }

                var classes = await _unitOfWork.ClassRepository.GetAllAsync1(c =>
            c.CourseId == parsedCourseId &&
            c.Status == ClassStatusEnum.Active &&
            c.AgencyId == agencyId);

                var classViewModels = new List<ClassViewModel>();
                var now = DateTime.Now;
                // var classViewModels = new List<ClassViewModel>();
                foreach (var c in classes)
                {
                    var allSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == c.Id);
                    var sortedSchedules = allSchedules.OrderBy(cs => cs.Date).ToList();

                    /*  if (!sortedSchedules.Any())
                      {
                          continue;
                      }*/

                    var totalSessions = sortedSchedules.Count;
                    var completedSessions = sortedSchedules.Count(s => s.Date < DateTime.Now);
                    var completionPercentage = (double)completedSessions / totalSessions * 100;

                    if (completionPercentage > 10)
                    {
                        continue;
                    }

                    var earliestSchedule = sortedSchedules.FirstOrDefault();
                    var instructorIds = await _unitOfWork.ClassRoomRepository.GetInstructorUserIdsByClassIdAsync(c.Id);

                    string instructorName = null;
                    if (instructorIds.Any())
                    {
                        var instructor = await _userManager.FindByIdAsync(instructorIds.First().ToString());
                        instructorName = instructor?.FullName;
                    }

                    var startDate = earliestSchedule?.Date;
                    var slot = earliestSchedule != null && earliestSchedule.SlotId.HasValue
                        ? await _unitOfWork.SlotRepository.GetByIdAsync(earliestSchedule.SlotId.Value)
                        : null;
                    int daysElapsed = 0;
                    if (startDate.HasValue && startDate.Value <= DateTime.Now)
                    {
                        daysElapsed = sortedSchedules.Count(s => s.Date <= DateTime.Now);
                    }
                    var classViewModel = new ClassViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Capacity = c.Capacity,
                        CurrentEnrollment = c.CurrentEnrollment,
                        InstructorName = instructorName,
                        CourseName = c.Course?.Name,
                        DayOfWeek = c.DayOfWeek + "-" + slot?.StartTime + "-" + slot?.EndTime,
                        StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null,
                        DaysElapsed = daysElapsed,
                        CourseId = c.CourseId
                    };

                    classViewModels.Add(classViewModel);
                }

                response = ResponseHandler.Success(classViewModels, "Lấy danh sách lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<ClassViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<ClassViewModel>>> GetAllClassByCourseIdandAgencyId(string courseId, Guid agencid)
        {
            var response = new ApiResponse<List<ClassViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                var agencyId = agencid;
                if (!Guid.TryParse(courseId, out Guid parsedCourseId))
                {
                    return ResponseHandler.Success<List<ClassViewModel>>(null, "CourseId không hợp lệ.");
                }

                var classes = await _unitOfWork.ClassRepository.GetAllAsync1(c =>
            c.CourseId == parsedCourseId &&
            c.Status == ClassStatusEnum.Active &&
            c.AgencyId == agencyId);

                var classViewModels = new List<ClassViewModel>();
                var now = DateTime.Now;
                // var classViewModels = new List<ClassViewModel>();
                foreach (var c in classes)
                {
                    var allSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == c.Id);
                    var sortedSchedules = allSchedules.OrderBy(cs => cs.Date).ToList();

                    /*  if (!sortedSchedules.Any())
                      {
                          continue;
                      }*/

                    var totalSessions = sortedSchedules.Count;
                    var completedSessions = sortedSchedules.Count(s => s.Date < DateTime.Now);
                    var completionPercentage = (double)completedSessions / totalSessions * 100;

                    if (completionPercentage > 10)
                    {
                        continue;
                    }

                    var earliestSchedule = sortedSchedules.FirstOrDefault();
                    var instructorIds = await _unitOfWork.ClassRoomRepository.GetInstructorUserIdsByClassIdAsync(c.Id);

                    string instructorName = null;
                    if (instructorIds.Any())
                    {
                        var instructor = await _userManager.FindByIdAsync(instructorIds.First().ToString());
                        instructorName = instructor?.FullName;
                    }

                    var startDate = earliestSchedule?.Date;
                    var slot = earliestSchedule != null && earliestSchedule.SlotId.HasValue
                        ? await _unitOfWork.SlotRepository.GetByIdAsync(earliestSchedule.SlotId.Value)
                        : null;
                    int daysElapsed = 0;
                    if (startDate.HasValue && startDate.Value <= DateTime.Now)
                    {
                        daysElapsed = sortedSchedules.Count(s => s.Date <= DateTime.Now);
                    }
                    var classViewModel = new ClassViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Capacity = c.Capacity,
                        CurrentEnrollment = c.CurrentEnrollment,
                        InstructorName = instructorName,
                        CourseName = c.Course?.Name,
                        DayOfWeek = c.DayOfWeek,
                        SlotStart = slot.StartTime.ToString(),
                        SlotEnd= slot.EndTime.ToString(),
                        StartDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null,
                        DaysElapsed = daysElapsed,
                        CourseId = c.CourseId,
                        TotalLessons=c.Course.NumberOfLession
                    };

                    classViewModels.Add(classViewModel);
                }

                response = ResponseHandler.Success(classViewModels, "Lấy danh sách lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<ClassViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<ClassByLoginViewModel>>> GetAllClassByLogin()
        {

            try
            {
                var currentUserId = _claimsService.GetCurrentUserId.ToString();
                var classes = await _unitOfWork.ClassRepository.GetClassesByUserIdAsync(currentUserId);

                var classViewModels = classes.Select(c => new ClassByLoginViewModel
                {
                    ClassId = c.Id,
                    ClassName = c.Name,
                    CourseId = c.CourseId,
                }).ToList();

                return ResponseHandler.Success<List<ClassByLoginViewModel>>(classViewModels, "Successfully retrieved classes");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<List<ClassByLoginViewModel>>(ex.Message);
            }
        }
        private async Task<AddQuizAndAssignmentModel> CreateQuizAndAssignmentForClass(Guid courseId)
        {
            var listQuiz = new List<Quiz>();
            var listAssignment = new List<Assignment>();
            var assessments = await _unitOfWork.AssessmentRepository.GetAssessmentsByCourseIdAsync(courseId);

            foreach (var assessment in assessments)
            {
                switch (assessment.Type)
                {
                    case AssessmentTypeEnum.Quiz:
                        {
                            for (int i = 0; i < assessment.Quantity; i++)
                            {
                                Quiz quiz = new Quiz()
                                {
                                    StartTime = DateTime.Now.AddYears(1),
                                    Quantity = 0,
                                    Title = "Bài kiểm tra định kì số " + (i + 1),
                                    Description = "Bài kiểm tra định kì số " + (i + 1),
                                    Type = QuizTypeEnum.Compulsory,
                                    AssessmentId = assessment.Id
                                };
                                listQuiz.Add(quiz);
                            }
                            break;
                        }
                    case AssessmentTypeEnum.Assignment:
                        {
                            for (int i = 0; i < assessment.Quantity; i++)
                            {
                                Assignment assignment = new Assignment()
                                {
                                    StartTime = DateTime.Now.AddYears(1),
                                    EndTime = DateTime.Now.AddYears(1).AddHours(20),
                                    Title = "Bài tập định kì số " + (i + 1),
                                    Description = "Bài tập định kì số " + (i + 1),
                                    Type = AssigmentTypeEnum.Compulsory,
                                    AssessmentId = assessment.Id
                                };
                                listAssignment.Add(assignment);
                            }
                            break;
                        }
                    case AssessmentTypeEnum.FinalExam:
                        {
                            for (int i = 0; i < assessment.Quantity; i++)
                            {
                                Quiz quiz = new Quiz()
                                {
                                    StartTime = DateTime.Now.AddYears(1),
                                    Quantity = 0,
                                    Title = "Bài kiểm cuối khóa số " + (i + 1),
                                    Description = "Bài kiểm cuối khóa số " + (i + 1),
                                    Type = QuizTypeEnum.Compulsory,
                                    AssessmentId = assessment.Id
                                };
                                listQuiz.Add(quiz);
                            }
                            break;
                        }
                }

            }

            var result = new AddQuizAndAssignmentModel() 
            {
                Assignments = listAssignment,
                Quizzes = listQuiz,
            };
            return result;
        }

    }
}

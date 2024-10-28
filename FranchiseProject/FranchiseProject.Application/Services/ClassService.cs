using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);
                }
                if (!string.IsNullOrEmpty(model.InstructorId))
                {
                    var instructor = await _userManager.FindByIdAsync(model.InstructorId);
                    if (instructor == null || !await _userManager.IsInRoleAsync(instructor, "Instructor"))
                    {
                        return ResponseHandler.Failure<bool>("Người hướng dẫn không hợp lệ hoặc không có vai trò là 'Instructor'!");
                    }
                }
                //Check List Học sinh có có đủ điều kiện không ==>Student đó phải có Status là waitlisted 
                if (model.StudentId == null || model.StudentId.Count == 0)
                {
                    return ResponseHandler.Failure<bool>("Danh sách học sinh không hợp lệ!");
                }
                var classWithSameName = await _unitOfWork.ClassRepository.GetFirstOrDefaultAsync(c => c.Name == model.Name && !c.IsDeleted);
                if (classWithSameName != null)
                {
                    return ResponseHandler.Failure<bool>($"Tên lớp '{model.Name}' đã tồn tại!");
                }
                if (model.StudentId.Count > model.Capacity)
                {
                    return ResponseHandler.Failure<bool>($"Số lượng học sinh vượt quá sức chứa lớp học! Sức chứa tối đa: {model.Capacity}.");
                }
                var invalidCourseRegistrations = await _unitOfWork.RegisterCourseRepository.GetAllAsync(rc =>
                    model.StudentId.Contains(rc.UserId) && rc.CourseId != Guid.Parse(model.CourseId) && rc.StudentCourseStatus != StudentCourseStatusEnum.NotStudied);


                // Kiểm tra trạng thái của từng học sinh
                var students = await _unitOfWork.UserRepository.GetAllAsync(u => model.StudentId.Contains(u.Id));
                var waitlistedStudents = await _unitOfWork.ClassRoomRepository.GetWaitlistedStudentsAsync(model.StudentId);
                if (waitlistedStudents.Count == 0)
                {

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
                var parsedCourseId = Guid.Parse(model.CourseId);

                var registerCourses = await _unitOfWork.RegisterCourseRepository.GetAllAsync(rc =>
                    model.StudentId.Contains(rc.UserId) && rc.CourseId == parsedCourseId);

                foreach (var registerCourse in registerCourses)
                {
                    registerCourse.StudentCourseStatus = StudentCourseStatusEnum.CurrentlyStudying;
                    await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                }

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
                await _unitOfWork.ClassRepository.AddAsync(newClass);
                if (model.InstructorId != null)
                {
                    var classRoom1 = new ClassRoom
                    {
                        ClassId = newClass.Id,
                        UserId = model.InstructorId,
                    };

                    await _unitOfWork.ClassRoomRepository.AddAsync(classRoom1);
                }
                foreach (var student in waitlistedStudents)
                {

                    var classRoom = new ClassRoom
                    {
                        ClassId = newClass.Id,
                        UserId = student.Id,
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
                    return ResponseHandler.Failure<bool>("Không tìm thấy lớp học.");
                }
                var isNameExist = await _unitOfWork.ClassRepository.AnyAsync(c => c.Name == model.Name && !c.IsDeleted && c.Id != classId);
                if (isNameExist)
                {
                    return ResponseHandler.Failure<bool>("Tên lớp học đã tồn tại.");
                }

                existingClass.Name = model.Name;
                existingClass.Capacity = model.Capacity;
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
                    return ResponseHandler.Failure<Pagination<ClassViewModel>>("User hoặc Agency không khả dụng!");
                }

                Expression<Func<Class, bool>> filter = c =>
                    (string.IsNullOrEmpty(filterClassModel.Name) || c.Name.Contains(filterClassModel.Name)) &&
                    (!filterClassModel.Status.HasValue || c.Status == filterClassModel.Status) &&
                    (string.IsNullOrEmpty(filterClassModel.CourseId) || c.CourseId == Guid.Parse(filterClassModel.CourseId)) &&
                    (c.AgencyId == userCurrent.AgencyId);

                var classes = await _unitOfWork.ClassRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "Course",
                    pageIndex: filterClassModel.PageIndex,
                    pageSize: filterClassModel.PageSize
                );

                var classViewModels = new Pagination<ClassViewModel>
                {

                    Items = classes.Items.Select(c => new ClassViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Capacity = c.Capacity,
                        CurrentEnrollment = c.CurrentEnrollment,

                        CourseName = c.Course.Name,
                        Status = c.Status.Value
                    }).ToList(),
                    TotalItemsCount = classes.TotalItemsCount,
                    PageIndex = classes.PageIndex,
                    PageSize = classes.PageSize
                };

                response = ResponseHandler.Success(classViewModels, "Lọc lớp học thành công!");
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
                    return ResponseHandler.Failure<ClassViewModel>("Không tìm thấy lớp học!");
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
                    return ResponseHandler.Failure<bool>("Không tìm thấy lớp học!");
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
        public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetClassDetailAsync(string id)
        {
            var response = new ApiResponse<Pagination<ClassStudentViewModel>>();
            try
            {
                var classId = Guid.Parse(id);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classId);
                if (classEntity == null)
                {
                    return ResponseHandler.Failure<Pagination<ClassStudentViewModel>>("Không tìm thấy lớp học!");
                }

                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classEntity.Id);
                var courseEntity = classEntity.CourseId.HasValue
                    ? await _unitOfWork.CourseRepository.GetByIdAsync(classEntity.CourseId.Value)
                    : null;

                var studentInfo = new List<StudentClassViewModel>();


                foreach (var cr in classRooms)
                {
                    var user = await _userManager.FindByIdAsync(cr.UserId);
                    studentInfo.Add(new StudentClassViewModel
                    {
                        UserId = cr.UserId,
                        StudentName = user?.FullName,
                        DateOfBirth = user?.DateOfBirth,
                        URLImage = user?.URLImage
                    });
                }
                var classSchedules = await _unitOfWork.SlotRepository
                     .GetAllAsync1(cs => cs.ClassId == classEntity.Id);

                var slotViewModels = new List<SlotViewModel>();
                string instructorName = string.Empty;
                foreach (var cr in classRooms)
                {
                    var user = await _userManager.FindByIdAsync(cr.UserId);
                    studentInfo.Add(new StudentClassViewModel
                    {
                        UserId = cr.UserId,
                        StudentName = user?.FullName,
                        DateOfBirth = user?.DateOfBirth,
                        URLImage = user?.URLImage
                    });
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Instructor"))
                    {
                        instructorName = user.FullName;
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
                    CourseId = classEntity.CourseId,
                    CourseName = courseEntity?.Name,
                    InstructorName = instructorName,
                    StudentInfo = studentInfo,
                    SlotViewModels = slotViewModels.FirstOrDefault()
                };

                var pagination = new Pagination<ClassStudentViewModel>
                {
                    Items = new List<ClassStudentViewModel> { classDetail },
                    TotalItemsCount = 1,
                    PageIndex = 1,
                    PageSize = 1
                };

                response = ResponseHandler.Success(pagination, "Lấy thông tin lớp học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<ClassStudentViewModel>>(ex.Message);
            }
            return response;
        }


        public async Task<ApiResponse<bool>> AddStudentAsync(string classId, AddStudentViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classGuid = Guid.Parse(classId);
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(classGuid);
                if (classEntity == null)
                {
                    return ResponseHandler.Failure<bool>("Lớp học không tồn tại!");
                }
                if (classEntity.Status != ClassStatusEnum.Inactive)
                {
                    return ResponseHandler.Failure<bool>("Lớp học phải ở trạng thái 'Inactive' để thêm học sinh!");
                }
                var existingClassRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classGuid);
                if (classEntity.CurrentEnrollment + model.StudentId.Count > classEntity.Capacity)
                {
                    return ResponseHandler.Failure<bool>("Số lượng học sinh vượt quá sức chứa của lớp!");
                }

                foreach (var studentId in model.StudentId)
                {
                    var student = await _userManager.FindByIdAsync(studentId);
                    if (student == null)
                    {
                        return ResponseHandler.Failure<bool>($"Học sinh với ID {studentId} không tồn tại!");
                    }
                    if (student.StudentStatus != StudentStatusEnum.Waitlisted)
                    {
                        return ResponseHandler.Failure<bool>($"Học sinh với ID {student.FullName} phải có trạng thái 'Waitlisted' để thêm vào lớp!");
                    }
                    if (existingClassRooms.All(cr => cr.UserId != studentId))
                    {
                        var classRoom = new ClassRoom
                        {
                            UserId = studentId,
                            ClassId = classGuid
                        };
                        student.StudentStatus = StudentStatusEnum.Enrolled;
                        var updateStudentResult = await _userManager.UpdateAsync(student);
                        if (!updateStudentResult.Succeeded)
                        {
                            throw new Exception($"Cập nhật trạng thái học sinh {student.FullName} thất bại!");
                        }
                        await _unitOfWork.ClassRoomRepository.AddAsync(classRoom);
                    }
                }
                classEntity.CurrentEnrollment = existingClassRooms.Count + model.StudentId.Count;
                _unitOfWork.ClassRepository.Update(classEntity);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Thêm học sinh thất bại!");

                response = ResponseHandler.Success(true, "Thêm học sinh thành công!");
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
                    return ResponseHandler.Failure<bool>("Lớp học không tồn tại!");
                }

                var classRoom = await _unitOfWork.ClassRoomRepository.GetFirstOrDefaultAsync(cr => cr.UserId == studentId && cr.ClassId == Guid.Parse(classId));
                if (classRoom == null)
                {
                    return ResponseHandler.Failure<bool>("Học sinh không có trong lớp học này!");
                }

                var student = await _userManager.FindByIdAsync(studentId);
                if (student == null)
                {
                    return ResponseHandler.Failure<bool>("Học sinh không tồn tại!");
                }
                student.StudentStatus = StudentStatusEnum.Waitlisted;
                var updateStudentResult = await _userManager.UpdateAsync(student);
                if (!updateStudentResult.Succeeded)
                {
                    throw new Exception("Cập nhật trạng thái học sinh thất bại!");
                }
                await _unitOfWork.ClassRoomRepository.DeleteAsync(classRoom);
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(cr =>
                    cr.UserId == studentId && cr.CourseId == classEntity.CourseId && cr.StudentCourseStatus == StudentCourseStatusEnum.CurrentlyStudying);

                if (registerCourse != null)
                {
                    registerCourse.StudentCourseStatus = StudentCourseStatusEnum.NotStudied;
                    await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                }
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
                    return ResponseHandler.Failure<bool>("Lớp không tồn tại hoặc đã bị xóa!");
                }


                classEntity.IsDeleted = true;
                _unitOfWork.ClassRepository.Update(classEntity);
                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classID);
                foreach (var classRoom in classRooms)
                {
                    await _unitOfWork.ClassRoomRepository.DeleteAsync(classRoom);
                }
                foreach (var classRoom in classRooms)
                {
                    var user = await _userManager.FindByIdAsync(classRoom.UserId);
                    if (user != null)
                    {

                        user.StudentStatus = StudentStatusEnum.Waitlisted;
                        _userManager.UpdateAsync(user);
                    }
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
        public async Task<ApiResponse<List<ClassScheduleViewModel>>> GetClassSchedulesByClassIdAsync(string classId)
        {
            var response = new ApiResponse<List<ClassScheduleViewModel>>();
            try
            {
                var classGuidId = Guid.Parse(classId);
                // Lấy tất cả ClassSchedules cho Class với ID đã cho
                var classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllAsync1(cs => cs.ClassId == classGuidId);
                var classE=await _unitOfWork.ClassRepository.GetByIdAsync(classGuidId);
                if (classSchedules == null || !classSchedules.Any())
                {
                    return ResponseHandler.Failure<List<ClassScheduleViewModel>>("Không tìm thấy lịch học cho lớp này!");
                }
                var classScheduleViewModels = new List<ClassScheduleViewModel>();

                foreach (var schedule in classSchedules)
                {
                    var slot = await _unitOfWork.SlotRepository.GetByIdAsync(schedule.SlotId.Value);

                    classScheduleViewModels.Add(new ClassScheduleViewModel
                    {
                        Id = schedule.Id.ToString(),
                        Room = schedule.Room,
                        ClassName = classE.Name,
                        SlotName = slot?.Name, // Có thể null nếu không tìm thấy Slot
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



    }
}

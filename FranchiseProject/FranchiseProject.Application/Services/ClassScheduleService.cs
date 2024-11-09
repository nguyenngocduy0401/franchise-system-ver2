using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.AttendanceViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ClassScheduleService : IClassScheduleService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateClassScheduleViewModel> _validator1;
        private readonly IValidator<CreateClassScheduleDateRangeViewModel> _validator2;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ClassScheduleService(IEmailService emailService, IValidator<CreateClassScheduleDateRangeViewModel> validator2, IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateClassScheduleViewModel> validator1, UserManager<User> userManager, IHubContext<NotificationHub> hubContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator1 = validator1;
            _validator2 = validator2;
            _userManager = userManager;
            _emailService = emailService;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator1.ValidateAsync(createClassScheduleViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                DateTime? date = DateTime.Parse(createClassScheduleViewModel.Date);
                var existingSchedule = await _unitOfWork.ClassScheduleRepository
                        .GetExistingScheduleAsync((DateTime)date, createClassScheduleViewModel.Room, Guid.Parse(createClassScheduleViewModel.SlotId));

                if (existingSchedule != null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = $"Đã tồn tại lớp học .";
                    return response;
                }
                var classSchedule = _mapper.Map<ClassSchedule>(createClassScheduleViewModel);
                await _unitOfWork.ClassScheduleRepository.AddAsync(classSchedule);
                var classE = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(createClassScheduleViewModel.ClassId));
                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == classSchedule.ClassId);
                var userIds = classRooms.Select(cr => cr.UserId).Distinct().ToList();
                foreach (var userId in userIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        var emailMessage = EmailTemplate.ClassScheduleCreated(user.Email, user.UserName,classE.Name);
                        bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                        var attendance = new Attendance
                        {
                        Status=AttendanceStatusEnum.NotStarted,
                            ClassScheduleId = classSchedule.Id,
                            UserId = userId
                        };
                        await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                        if (!emailSent)
                        {
                            response.Message += $" Lỗi khi gửi email đến {user.Email}.";
                        }
                    }
                    await _hubContext.Clients.User(userId.ToString())
                        .SendAsync("ReceivedNotification", $"Lịch học mới đã được tạo cho lớp {classE.Name} và sẻ bắt đầu từ ngày {classSchedule.Date}.");
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo  thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync(CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator2.ValidateAsync(createClassScheduleDateRangeViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var classEntity = await _unitOfWork.ClassRepository.GetExistByIdAsync(Guid.Parse(createClassScheduleDateRangeViewModel.ClassId));
                if (classEntity == null)
                {
                    return ResponseHandler.Success<bool>(false,"Lớp học không tồn tại!");
                }


                var courseEntity = await _unitOfWork.CourseRepository.GetByIdAsync(classEntity.CourseId.Value);
                if (courseEntity == null)
                {
                    return ResponseHandler.Success<bool>(false, "Khóa học không tồn tại!");
                }
                int numberOfLessons = courseEntity.NumberOfLession.Value;
                var selectedDaysOfWeek = new List<DayOfWeekEnum>();

                var inputDaysOfWeek = createClassScheduleDateRangeViewModel.dayOfWeeks; // Là danh sách các string

                foreach (var day in inputDaysOfWeek)
                {
                    if (Enum.TryParse(day.ToString(), out DayOfWeekEnum dayEnum))
                    {
                        selectedDaysOfWeek.Add(dayEnum);
                    }
                    else
                    {
                        response.Data = false;
                        response.isSuccess = true;
                        response.Message = $"Ngày '{day}' không hợp lệ!";
                        return response;
                    }
                }

                var startDate = DateTime.Now;
                var createdSchedules = new List<DateTime>();
                var classSchedules = new List<ClassSchedule>();
                int createdLessons = 0;

                while (createdLessons < numberOfLessons)
                {
                    if (selectedDaysOfWeek.Contains((DayOfWeekEnum)startDate.DayOfWeek))
                    {
                        var existingSchedule = await _unitOfWork.ClassScheduleRepository
                            .GetExistingScheduleAsync(startDate, createClassScheduleDateRangeViewModel.Room, Guid.Parse(createClassScheduleDateRangeViewModel.SlotId));

                        if (existingSchedule != null)
                        {
                            response.Data = false;
                            response.isSuccess = true;
                            response.Message = $"Lịch học đã tồn tại vào ngày {startDate:yyyy-MM-dd}, phòng {createClassScheduleDateRangeViewModel.Room}, slot {createClassScheduleDateRangeViewModel.SlotId}!";
                            return response;
                        }

                        var classSchedule = new ClassSchedule
                        {
                            ClassId = Guid.Parse(createClassScheduleDateRangeViewModel.ClassId),
                            SlotId = Guid.Parse(createClassScheduleDateRangeViewModel.SlotId),
                            Date = startDate,
                            Room = createClassScheduleDateRangeViewModel.Room
                        };

                        await _unitOfWork.ClassScheduleRepository.AddAsync(classSchedule);
                        classSchedules.Add(classSchedule);
                        createdSchedules.Add(startDate);
                        createdLessons++;
                    }

                    startDate = startDate.AddDays(1);
                }
                var dateClass = "";
                foreach (var date in createClassScheduleDateRangeViewModel.dayOfWeeks) { dateClass = dateClass + "  " + date.ToString(); }
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(Guid.Parse(createClassScheduleDateRangeViewModel.SlotId));
                var classE = await _unitOfWork.ClassRepository.GetByIdAsync(Guid.Parse(createClassScheduleDateRangeViewModel.ClassId));
                classE.DayOfWeek = dateClass;
                _unitOfWork.ClassRepository.Update(classE);
                var students = await _unitOfWork.ClassRepository.GetStudentsByClassIdAsync(Guid.Parse(createClassScheduleDateRangeViewModel.ClassId));
                foreach (var schedule in classSchedules)
                {
                    foreach (var student in students)
                    {
                        var attendance = new Attendance
                        {
                            Status =AttendanceStatusEnum.NotStarted,
                            ClassScheduleId = schedule.Id,
                            UserId = student.Id
                        };

                        await _unitOfWork.AttendanceRepository.AddAsync(attendance);
                    }
                }
                var classRoom= await _unitOfWork.ClassRoomRepository.GetAllAsync(rc=>rc.ClassId==Guid.Parse(createClassScheduleDateRangeViewModel.ClassId));
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Tạo lịch học thất bại!");

                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo lịch học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        public async Task<ApiResponse<bool>> DeleteClassScheduleByIdAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy slot!";
                    return response;
                }
                _unitOfWork.ClassScheduleRepository.SoftRemove(classSchedule);
                response.Message = "Xoá class schedule học thành công!";


                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete fail!");
                response.Data = true;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<List<ClassScheduleViewModel>>> FilterClassScheduleAsync(FilterClassScheduleViewModel filterClassScheduleViewModel)
        {
            var response = new ApiResponse<List<ClassScheduleViewModel>>();
            try
            {
                DateTime? start = null;
                DateTime? end = null;

                if (!string.IsNullOrEmpty(filterClassScheduleViewModel.StartDate))
                {
                    start = DateTime.Parse(filterClassScheduleViewModel.StartDate);
                }

                if (!string.IsNullOrEmpty(filterClassScheduleViewModel.EndDate))
                {
                    end = DateTime.Parse(filterClassScheduleViewModel.EndDate);
                }
                Expression<Func<ClassSchedule, bool>> filter = s =>
                    (!start.HasValue || s.Date.Value.Date >= start.Value.Date) &&
                    (!end.HasValue || s.Date.Value.Date <= end.Value.Date);
                var schedules = await _unitOfWork.ClassScheduleRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "Class,Slot"
                );

                var scheduleViewModels = schedules.Items.Select(s => new ClassScheduleViewModel
                {
                    Id = s.Id.ToString(),
                    Room = s.Room,
                    ClassId=s.ClassId,
                    ClassName = s.Class.Name,
                    SlotName = s.Slot.Name,
                    Date = s.Date?.ToString("yyyy-MM-dd"),
                    StartTime = s.Slot.StartTime?.ToString(@"hh\:mm"),
                    EndTime = s.Slot.EndTime?.ToString(@"hh\:mm")
                }).ToList();

                response.Data = scheduleViewModels; 
                response.isSuccess = true;
                response.Message = "Lấy danh sách ClassSchedule thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }


        public async Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id)
        {
            var response = new ApiResponse<ClassScheduleViewModel>();
            try
            {
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(Guid.Parse(id));
                if (classSchedule == null)
                {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy class schedule!";
                    return response;
                }
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(classSchedule.SlotId.Value);
                var class1 = await _unitOfWork.ClassRepository.GetByIdAsync(classSchedule.ClassId.Value);
                var clasScheduleViewModel = _mapper.Map<ClassScheduleViewModel>(classSchedule);
                clasScheduleViewModel.ClassId = classSchedule.ClassId.Value;
                clasScheduleViewModel.ClassName = class1.Name;
                clasScheduleViewModel.StartTime = slot.StartTime.ToString();
                clasScheduleViewModel.EndTime = slot.EndTime.ToString();
                response.Data = clasScheduleViewModel;
                response.isSuccess = true;
                response.Message = "tìm class schedule học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateClassScheduleAsync(CreateClassScheduleViewModel updateClassScheduleViewModel, string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator1.ValidateAsync(updateClassScheduleViewModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(Guid.Parse(id));
                _mapper.Map(updateClassScheduleViewModel, classSchedule);
                _unitOfWork.ClassScheduleRepository.Update(classSchedule);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Cập nhật thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteClassSheduleAllByClassIdAsync(string classId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var currentDate=DateTime.Now;
                
               
                var classSchedules = await _unitOfWork.ClassScheduleRepository.GetAllClassScheduleAsync(cs => cs.ClassId == Guid.Parse(classId));
                var classRooms = await _unitOfWork.ClassRoomRepository.GetAllAsync(cr => cr.ClassId == Guid.Parse(classId));
                var userIds = classRooms.Select(cr => cr.UserId).Distinct().ToList();
                var classE = await _unitOfWork.ClassRepository.GetExistByIdAsync(Guid.Parse(classId));
                var startDate = await _unitOfWork.ClassScheduleRepository.GetEarliestClassScheduleByClassIdAsync(Guid.Parse(classId));
                if (currentDate > startDate.Date)
                {
                    return ResponseHandler.Success(false, "Không thể xóa lịch học vì lớp học đã hoạt động !");
                }
                    if (!classSchedules.Any())
                {
                    return ResponseHandler.Success(false, "Không có lịch học nào để xóa.");
                }
                foreach (var classSchedule in classSchedules)
                {
                    var attendanceRecords = await _unitOfWork.AttendanceRepository.GetAllAsync(a => a.ClassScheduleId == classSchedule.Id);
                    if (attendanceRecords.Any())
                    {
                        _unitOfWork.AttendanceRepository.DeleteRange(attendanceRecords);
                    }
                }
                _unitOfWork.ClassScheduleRepository.HardRemoveRange( classSchedules);
                await _unitOfWork.SaveChangeAsync();
                foreach (var userId in userIds)
                {
         
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        var emailMessage = EmailTemplate.ClassScheduleChange(user.Email, user.UserName, classE.Name);
                        bool emailSent = await _emailService.SendEmailAsync(emailMessage);
                        if (!emailSent)
                        {
                            return ResponseHandler.Success(false, "Lỗi khi gửi mail");
                        }
                    }
                  
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceivedNotification", $"Lịch học của lớp {classE.Name} đã bị thay đổi");
                }

                response = ResponseHandler.Success(true, "Xóa tất cả lịch học của lớp thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>($"Lỗi khi xóa lịch học: {ex.Message}");
            }
            return response;
        }
        public async Task<ApiResponse<ClassScheduleDetailViewModel>>GetClassScheduleDetailAsync(Guid id)
        {
           
                var response = new ApiResponse<ClassScheduleDetailViewModel>();
                try
                {
                    var classSchedule = await _unitOfWork.ClassScheduleRepository.GetClassScheduleWithDetailsAsync(id);

                    if (classSchedule == null)
                    {
                        return ResponseHandler.Success<ClassScheduleDetailViewModel>(null,"Không tìm thấy lịch học.");
                    }
                    var numberOfStudents = classSchedule.Attendances?.Count ?? 0;
                    var studentInfos = classSchedule.Attendances?
                        .Select(a => new StudentClassScheduleViewModel
                        {
                            UserName = a.User.UserName,
                            UserId = a.User.Id,
                            StudentName = a.User.FullName,
                            DateOfBirth = a.User.DateOfBirth,
                            URLImage = a.User.URLImage,
                            AttendanceStatus=a.Status

                        }).ToList();
                    var classScheduleDetail = new ClassScheduleDetailViewModel
                    {
                        Id = classSchedule.Id.ToString(),
						Date = classSchedule.Date?.ToString("yyyy-MM-dd"),
                        StartTime = classSchedule.Slot?.StartTime,
                        EndTime = classSchedule.Slot?.EndTime,
                        NumberOfStudent = numberOfStudents,
                        StudentInfo = studentInfos,
                        CourseCode = classSchedule.Class.Course.Code,
						ClassName= classSchedule.Class.Name
					};

                    response = ResponseHandler.Success(classScheduleDetail, "Lấy chi tiết lịch học thành công!");
                }
                catch (Exception ex)
                {
                    response = ResponseHandler.Failure<ClassScheduleDetailViewModel>($"Lỗi khi lấy chi tiết lịch học: {ex.Message}");
                }
                return response;
            }
    }
}

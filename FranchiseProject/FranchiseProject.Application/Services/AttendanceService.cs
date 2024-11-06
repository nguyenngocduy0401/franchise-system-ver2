using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace FranchiseProject.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AttendanceService(IEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork, UserManager<User> userManager, IHubContext<NotificationHub> hubContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
          
            _userManager = userManager;
            _emailService = emailService;
            _hubContext = hubContext;
        }
        public async Task<ApiResponse<bool>> MarkAttendanceByClassScheduleAsync(Guid classScheduleId, List<string> studentIds)
        {
            var response = new ApiResponse<bool>();
            try
            {
                

                var attendances = await _unitOfWork.AttendanceRepository.GetAllAsync(a => a.ClassScheduleId == classScheduleId);
                var classSchedule = await _unitOfWork.ClassScheduleRepository.GetByIdAsync(classScheduleId);
                if (classSchedule.Date.Value.Date != DateTime.Today)
                {
                    return ResponseHandler.Success<bool>(false,"Lịch học không phải ngày hôm nay, không thể điểm danh.");
                }

                var currentStudentIds = attendances.Select(a => a.UserId).ToList();

                foreach (var studentId in studentIds)
                {
                    var attendance = attendances.FirstOrDefault(a => a.UserId == studentId);

                    
                        attendance.Status = AttendanceStatusEnum.Present;
                       await _unitOfWork.AttendanceRepository.UpdateAsync(attendance);
                    
                }
                var absentStudents = attendances.Where(a => !studentIds.Contains(a.UserId)).ToList();
                foreach (var absentAttendance in absentStudents)
                {
                    absentAttendance.Status = AttendanceStatusEnum.Absent;
                    await _unitOfWork.AttendanceRepository.UpdateAsync(absentAttendance);
                }



                response = ResponseHandler.Success(true, "Điểm danh thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>($"Lỗi khi điểm danh: {ex.Message}");
            }

            return response;
        }

    }
}

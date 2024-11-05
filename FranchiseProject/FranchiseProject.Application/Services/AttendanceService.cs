using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Domain.Entity;
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
        public async Task<ApiResponse<bool>>CreateAttendace(Guid classScheduleId,List<string> studentIds)
        {
            var response = new ApiResponse<bool>();
            try
            {


            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.NotificationViewModel;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class NotificationService:INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IUnitOfWork unitOfWork, IClaimsService claimsService,IMapper mapper,IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

   

        public async Task<ApiResponse<bool>> CreateAndSendNotificationAsync(SendNotificationViewModel sendNotificationViewModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (sendNotificationViewModel.userIds == null || sendNotificationViewModel.userIds.Count == 0 || string.IsNullOrEmpty(sendNotificationViewModel.message))
                {
                    response.Data = true;
                    response.isSuccess = false;
                    response.Message = "Danh sách user hoặc message không hợp lệ.";
                    return response;
                }
                var notifications = new List<Notification>();
                foreach (var userId in sendNotificationViewModel.userIds)
                {
                    var notification = new Notification
                    {
                        SenderId = _claimsService.GetCurrentUserId.ToString(), 
                        ReceiverId = userId,
                        Message = sendNotificationViewModel.message,
                        CreationDate = DateTime.UtcNow,
                        IsRead = false
                    };
                    notifications.Add(notification);
                }
                await _unitOfWork.NotificationRepository.AddRangeAsync(notifications);
                await _unitOfWork.SaveChangeAsync();
                foreach (var userId in sendNotificationViewModel.userIds)
                {
                     var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountByUserIdAsync(userId);
                    // Gửi thông báo đến client
                    await _hubContext.Clients.User(userId).SendAsync("ReceivedNotification", sendNotificationViewModel.message);
                    
                    // Gửi số lượng thông báo chưa đọc đến client
                    await _hubContext.Clients.User(userId).SendAsync("UpdateUnreadNotificationCount", unreadCount);
                }

                response.Data = true;
                response.isSuccess = true;
                response.Message = "Gửi thông báo Thành Công !";
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<int>> GetUnreadNotificationCountAsync()
        {
            var response = new ApiResponse<int>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var count = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountByUserIdAsync(userId);
                var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountByUserIdAsync(userId);
                await _hubContext.Clients.User(userId).SendAsync("UpdateUnreadNotificationCount", unreadCount);
                response.Data = count;
                response.isSuccess = true;
                response.Message = "Truy xuất số thông báo chưa đọc thành công !";
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> MarkNotificationAsReadAsync(string notificationId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var id = Guid.Parse(notificationId);
                var userId = _claimsService.GetCurrentUserId.ToString();

                // Cập nhật số lượng thông báo chưa đọc cho người dùng thông qua SignalR
                var unreadCount = await _unitOfWork.NotificationRepository.GetUnreadNotificationCountByUserIdAsync(userId);
                await _hubContext.Clients.User(userId).SendAsync("UpdateUnreadCount", unreadCount);
                await _unitOfWork.NotificationRepository.MarkNotificationAsReadAsync(id);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "đánh dấu  thông báo đã đọc thành công !";
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    
        public async Task<ApiResponse<List<NotificationViewModel>>> GetUserNotificationsAsync()
        {
            var response = new ApiResponse<List<NotificationViewModel>>();
            try
            {
                var userId=_claimsService.GetCurrentUserId.ToString();

                var notifications = await _unitOfWork.NotificationRepository.GetNotificationsByUserIdAsync(userId); 
                var notificationViewModels = _mapper.Map<List<NotificationViewModel>>(notifications);
                
                response.Data = notificationViewModels;
                response.isSuccess = true;
                response.Message = "truy xuất thành công !";
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

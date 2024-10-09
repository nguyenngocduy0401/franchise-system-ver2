using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.NotificationViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse<bool>> CreateAndSendNotificationAsync(SendNotificationViewModel sendNotificationViewModel);
        Task<ApiResponse<int>> GetUnreadNotificationCountAsync();
        Task<ApiResponse<bool>> MarkNotificationAsReadAsync(string notificationId);
        Task<ApiResponse<List<NotificationViewModel>>> GetUserNotificationsAsync();
    }
}

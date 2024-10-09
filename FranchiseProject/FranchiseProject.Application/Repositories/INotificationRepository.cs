using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface INotificationRepository :IGenericRepository<Notification>
    {
        Task<Notification> GetNotificationByIdAsync(string id);
        Task SendNotificationsAsync(List<string> userIds, string message, string senderId);
        Task<int> GetUnreadNotificationCountByUserIdAsync(string userId);
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task<List<Notification>> GetNotificationsByUserIdAsync(string userId);
    }
}

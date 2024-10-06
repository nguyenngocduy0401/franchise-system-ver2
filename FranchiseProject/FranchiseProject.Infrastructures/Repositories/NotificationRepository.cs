using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Google;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
      
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationRepository(IHubContext<NotificationHub> hubContext,AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
            _hubContext= hubContext;
        }
        public async Task<Notification> GetNotificationByIdAsync(string id)
        {
            var userId = Guid.Parse(id);
            return await _dbContext.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receiver)
                .FirstOrDefaultAsync(n => n.Id == userId);
        }
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _dbContext.Notifications
        .Where(n => n.ReceiverId == userId)  
        .OrderBy(n => n.IsRead) 
        .ThenByDescending(n => n.CreationDate)  
        .ToListAsync();
        }
        public async Task SendNotificationsAsync(List<string> userIds, string message, string senderId)
        {
            var notifications = new List<Notification>();

            foreach (var userId in userIds)
            {
                var notification = new Notification
                {
                    SenderId = senderId,
                    ReceiverId = userId,
                    Message = message,
                    CreationDate = DateTime.UtcNow,
                    IsRead = false
                };
                notifications.Add(notification);
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
            await _dbContext.Notifications.AddRangeAsync(notifications);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> GetUnreadNotificationCountByUserIdAsync(string userId)
        {
            return await _dbContext.Notifications
                                 .Where(n => n.ReceiverId == userId && !n.IsRead)
                                 .CountAsync();
        }
        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

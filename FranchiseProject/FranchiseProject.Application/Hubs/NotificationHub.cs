using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Hubs
{
    public class NotificationHub : Hub
    {
        // Lưu trữ danh sách các kết nối theo từng UserId
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;  // Lấy UserId từ Context.UserIdentifier, được thiết lập bởi CustomUserIdProvider

            if (!string.IsNullOrEmpty(userId))
            {
                // Lưu trữ ConnectionId với UserId
                UserConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with Connection ID: {Context.ConnectionId}");
            }
            else
            {
                Console.WriteLine($"UserIdentifier is null or empty for Connection ID: {Context.ConnectionId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceivedNotification", message);
        }

        public async Task UpdateUnreadNotificationCount(string userId, int unreadCount)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UserId cannot be null or empty.");
                return;
            }

            try
            {
                await Clients.User(userId).SendAsync("UpdateUnreadNotificationCount", unreadCount);
                Console.WriteLine($"Updated unread notification count for user {userId}: {unreadCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating unread notification count for user {userId}: {ex.Message}");
            }
        }
    }
}
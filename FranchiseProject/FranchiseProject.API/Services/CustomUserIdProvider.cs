using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FranchiseProject.API.Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            connection.GetHttpContext().Request.Query.TryGetValue("userId", out var userId);
            return userId;
        }
    }
}
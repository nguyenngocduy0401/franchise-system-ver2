using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.NotificationViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace FranchiseProject.API.Controllers
{
     [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext= hubContext;
        }

        [SwaggerOperation(Summary = "Admin tạo và gửi thông báo đến người dùng{Authorize= Manager,Admin,FranchiseManager,Instructor}")]
        [HttpPost("")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager  )] 
        public async Task<ApiResponse<bool>> CreateAndSendNotificationAsync([FromBody] SendNotificationViewModel model)
        {
            return await _notificationService.CreateAndSendNotificationAsync(model);
        }

        [SwaggerOperation(Summary = "Lấy tổng số thông báo chưa đọc của người dùng")]
        [HttpGet("")]
        [Authorize]
        public async Task<ApiResponse<int>> GetUnreadNotificationCountAsync()
        {
            return await _notificationService.GetUnreadNotificationCountAsync();
        }

        [SwaggerOperation(Summary = "Đánh dấu thông báo là đã đọc")]
        [HttpPost("{id}")]
        [Authorize] 
        public async Task<ApiResponse<bool>> MarkNotificationAsReadAsync(string id) { 
            return await _notificationService.MarkNotificationAsReadAsync(id);
        }

        [SwaggerOperation(Summary = "Lấy danh sách thông báo của người dùng")]
        [HttpGet("mine")]
        [Authorize]
        public async Task<ApiResponse<List<NotificationViewModel>>> GetUserNotificationsAsync()
        {
            return await _notificationService.GetUserNotificationsAsync();
        }
        [HttpPost("test")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationViewModel sendNotificationViewModel)
        {
            if (sendNotificationViewModel.userIds == null || sendNotificationViewModel.userIds.Count == 0 || string.IsNullOrEmpty(sendNotificationViewModel.message))
            {
                return BadRequest("Danh sách user hoặc message không hợp lệ.");
            }

            foreach (var userId in sendNotificationViewModel.userIds)
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", sendNotificationViewModel.message);
            }

            return Ok(new { success = true, message = "Thông báo đã được gửi thành công." });
        }
    }
}

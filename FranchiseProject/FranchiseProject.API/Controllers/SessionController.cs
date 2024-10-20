/*using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/sessions")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa buổi học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteSessionByIdAsync(Guid id)
        {
            return await _sessionService.DeleteSessionByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo buổi học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateSessionAsync(CreateSessionModel createSessionModel)
        {
            return await _sessionService.CreateSessionAsync(createSessionModel);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật buổi học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateSessionAsync(Guid id, UpdateSessionModel updateSessionModel)
        {
            return await _sessionService.UpdateSessionAsync(id, updateSessionModel);
        }
        [SwaggerOperation(Summary = "tìm kiếm buổi học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<SessionViewModel>> GetSessionByIdAsync(Guid id)
        {
            return await _sessionService.GetSessionByIdAsync(id);
        }
    }
}
*/
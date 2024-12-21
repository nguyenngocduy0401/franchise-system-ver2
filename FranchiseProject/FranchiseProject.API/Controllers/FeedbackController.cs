using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase

    {
        private IFeedbackService _feedbackService;
        public FeedbackController(IFeedbackService feedbackService) { _feedbackService = feedbackService; }

        [Authorize(Roles = AppRole.Student)]
        [SwaggerOperation(Summary = "tạo feedback {Authorize = Student }")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> CreateFeedBackAsync(CreateFeedBackViewModel model) => await _feedbackService.CreateFeedBackAsync(model);
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "lấy danh sách feedback {Authorize = SystemInstructor, Manager}")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<FeedBackViewModel>>> FilterFeedBackAsync(FilterFeedbackViewModel filterModel)
        {
            return await _feedbackService.FilterFeedBackAsync(filterModel);
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "lấy feedback bằng id {Authorize = SystemInstructor, Manager}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<FeedBackViewModel>> GetFeedBaByIdAsync(Guid id) => await _feedbackService.GetFeedBaByIdAsync(id);
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "xóa feedback bằng id {Authorize = SystemInstructor, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteFeedBackByIdAsync(Guid id) => await _feedbackService.DeleteFeedBackByIdAsync(id);
    }
}

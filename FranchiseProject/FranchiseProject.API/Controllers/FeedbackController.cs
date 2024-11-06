using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;
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

        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "lấy danh sách feedback {Authorize = SystemInstructor, Manager}")]
        [HttpGet("")]
        public async Task<ApiResponse<bool>> CreateFeedBackAsync(CreateFeedBackViewModel model)
        {
            return await _feedbackService.CreateFeedBackAsync(model);
        }


    }
}

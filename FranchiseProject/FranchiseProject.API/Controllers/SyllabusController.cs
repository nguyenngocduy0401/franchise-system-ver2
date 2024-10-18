using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/Syllabuses")]
    [ApiController]
    public class SyllabusController : ControllerBase
    {
        private readonly ISyllabusService _syllabusService;
        public SyllabusController(ISyllabusService syllabusService)
        {
            _syllabusService = syllabusService;
        }
        /*[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa đánh giá của khóa học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteSyllabusByIdAsync(Guid id)
        {
            return await _syllabusService.DeleteAssessmentByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo đánh giá của khóa học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateAssessmentAsync(CreateAssessmentModel createAssessmentModel)
        {
            return await _assessmentService.CreateAssessmentAsync(createAssessmentModel);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật đánh giá của khóa học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateAssessmentAsync(Guid id, UpdateAssessmentModel updateAssessmentModel)
        {
            return await _assessmentService.UpdateAssessmentAsync(id, updateAssessmentModel);
        }
        [SwaggerOperation(Summary = "tìm kiếm đánh giá của khóa học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<AssessmentViewModel>> GetAssessmentByIdAsync(Guid id)
        {
            return await _assessmentService.GetAssessmentByIdAsync(id);
        }*/
    }
}

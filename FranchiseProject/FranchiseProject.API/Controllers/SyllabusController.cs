using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
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
        /*[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]*/
        [SwaggerOperation(Summary = "xóa giáo trình của khóa học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteSyllabusByIdAsync(Guid id)
        {
            return await _syllabusService.DeleteSyllabusByIdAsync(id);
        }
        /*[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]*/
        [SwaggerOperation(Summary = "tạo giáo trình của khóa học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateSyllabusAsync(CreateSyllabusModel createSyllabusModel)
        {
            return await _syllabusService.CreateSyllabusAsync(createSyllabusModel);
        }
        /*[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]*/
        [SwaggerOperation(Summary = "cập nhật giáo trình của khóa học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateSyllabusAsync(Guid id, UpdateSyllabusModel updateSyllabusModel)
        {
            return await _syllabusService.UpdateSyllabusAsync(id, updateSyllabusModel);
        }
        [SwaggerOperation(Summary = "tìm kiếm giáo trình của khóa học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<SyllabusViewModel>> GetSyllabusByIdAsync(Guid id)
        {
            return await _syllabusService.GetSyllabusByIdAsync(id);
        }
    }
}

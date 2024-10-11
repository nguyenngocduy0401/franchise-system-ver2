using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/course-categories")]
    [ApiController]
    public class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService _courseCategoryService;
        public CourseCategoryController(ICourseCategoryService courseCategoryService)
        {
            _courseCategoryService = courseCategoryService;
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa category bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteCourseCategoryAsync(Guid id)
        {
            return await _courseCategoryService.DeleteCourseCategoryByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới category {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateCourseCategoryAsync(CreateCourseCategoryModel createCourseCategoryModel)
        {
            return await _courseCategoryService.CreateCourseCategoryAsync(createCourseCategoryModel);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật category {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateCourseCategoryAsync(Guid id, UpdateCourseCategoryModel updateCourseCategoryModel)
        {
            return await _courseCategoryService.UpdateCourseCategoryAsync(id, updateCourseCategoryModel);
        }
        [SwaggerOperation(Summary = "tìm category bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<CourseCategoryViewModel>> GetCourseCategoryByIdAsync(Guid id)
        {
            return await _courseCategoryService.GetCourseCategoryByIdAsync(id);
        }
        [SwaggerOperation(Summary = "lấy tất cả category")]
        [HttpGet()]
        public async Task<ApiResponse<List<CourseCategoryViewModel>>> GetAllCourseCategoryAsync()
        {
            return await _courseCategoryService.GetAllCourseCategoryAsync();
        }
        [SwaggerOperation(Summary = "tìm kiếm category {Authorize = Admin, Manager}")]
        [HttpGet("~/manager/api/v1/course-categories")]
        public async Task<ApiResponse<Pagination<CourseCategoryViewModel>>> FilterCourseCategoryAsync([FromQuery]FilterCourseCategoryModel filterCourseCategoryModel)
        {
            return await _courseCategoryService.FilterCourseCategoryAsync(filterCourseCategoryModel);
        }
    }
}
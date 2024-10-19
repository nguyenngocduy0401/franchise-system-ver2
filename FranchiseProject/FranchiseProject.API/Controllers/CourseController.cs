using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMaterialService _materialService;
        public CourseController(ICourseService courseService, IMaterialService materialService)
        {
            _courseService = courseService;
            _materialService = materialService;
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa khoá học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteCourseAsync(Guid id)
        {
            return await _courseService.DeleteCourseByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới khoá học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateCourseAsync(CreateCourseModel createCourseModel)
        {
            return await _courseService.CreateCourseAsync(createCourseModel);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật khoá học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateCourseAsync(Guid id, UpdateCourseModel updateCourseModel)
        {
            return await _courseService.UpdateCourseAsync(id, updateCourseModel);
        }
        [SwaggerOperation(Summary = "cập nhật tài nguyên của khoá học {Authorize = Admin, Manager}")]
        [HttpPost("{id}/materials")]
        public async Task<ApiResponse<bool>> CreateMaterialByCourseIdAsync(Guid id, List<CreateMaterialArrangeModel> createMaterialArrangeModel)
        {
            return await _materialService.CreateMaterialArangeAsync(id, createMaterialArrangeModel);
        }
        [SwaggerOperation(Summary = "tìm khoá học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid id)
        {
            return await _courseService.GetCourseByIdAsync(id);
        }
        [SwaggerOperation(Summary = "tìm kiếm khoá học")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<CourseViewModel>>> FilterCourseAsync([FromQuery] FilterCourseModel filterCourseModel)
        {
            return await _courseService.FilterCourseAsync(filterCourseModel);
        }
    }
}

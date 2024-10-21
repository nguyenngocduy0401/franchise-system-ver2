/*using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/course-materials")]
    [ApiController]
    public class CourseMaterialController : ControllerBase
    {
        private readonly ICourseMaterialService _courseMaterialService;
        public CourseMaterialController(ICourseMaterialService courseMaterialService)
        {
            _courseMaterialService = courseMaterialService;
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa tài nguyên học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteCourseMaterialByIdAsync(Guid id)
        {
            return await _courseMaterialService.DeleteCourseMaterialByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới tài nguyên {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateCourseMaterialAsync(CreateCourseMaterialModel createCourseMaterialModel)
        {
            return await _courseMaterialService.CreateCourseMaterialAsync(createCourseMaterialModel);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật tài nguyên {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateCourseMaterialAsync(Guid id, UpdateCourseMaterialModel updateCourseMaterialModel)
        {
            return await _courseMaterialService.UpdateCourseMaterialAsync(id, updateCourseMaterialModel);
        }
        [SwaggerOperation(Summary = "tìm tài nguyên bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<CourseMaterialViewModel>> GetCourseMaterialByIdAsync(Guid id)
        {
            return await _courseMaterialService.GetCourseMaterialByIdAsync(id);
        }

    }
}
*/
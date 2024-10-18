using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/materials")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        public MaterialController(IMaterialService materialService)
        {
            _materialService = materialService;
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa tài nguyên học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteMaterialByIdAsync(Guid id)
        {
            return await _materialService.DeleteMaterialByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới tài nguyên {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateMaterialAsync(CreateMaterialModel createMaterialModel)
        {
            return await _materialService.CreateMaterialAsync(createMaterialModel);
        }
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật tài nguyên {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateMaterialAsync(Guid id, UpdateMaterialModel updateMaterialModel)
        {
            return await _materialService.UpdateMaterialAsync(id, updateMaterialModel);
        }
        [SwaggerOperation(Summary = "tìm tài nguyên bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<MaterialViewModel>> GetMaterialByIdAsync(Guid id)
        {
            return await _materialService.GetMaterialByIdAsync(id);
        }
      
    }
}

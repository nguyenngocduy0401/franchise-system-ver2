using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.PackageViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/packages")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa gói nhượng quyền bằng id {Authorize = Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeletePackageByIdAsync(Guid id)
        {
            return await _packageService.DeletePackageByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo mới gói nhượng quyền{Authorize = Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreatePackageAsync(CreatePackageModel createPackageModel)
        {
            return await _packageService.CreatePackageAsync(createPackageModel);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật gói nhượng quyền {Authorize = Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdatePackageAsync(Guid id, UpdatePackageModel updatePackageModel)
        {
            return await _packageService.UpdatePackageAsync(id, updatePackageModel);
        }
        [SwaggerOperation(Summary = "tìm gói nhượng quyền bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<PackageViewModel>> GetSlotByIdAsync(Guid id)
        {
            return await _packageService.GetPackageByIdAsync(id);
        }
        /*[SwaggerOperation(Summary = "tìm kiếm slot")]
        [HttpGet("~/manager/api/v1/slots")]
        public async Task<ApiResponse<Pagination<SlotViewModel>>> FilterSlotAsync([FromQuery] FilterSlotModel filterSlotModel)
        {
            return await _slotService.FilterSlotAsync(filterSlotModel);
        }*/
        [SwaggerOperation(Summary = "lấy tất cả gói nhượng quyền cho khách hàng nhìn thấy")]
        [HttpGet()]
        public async Task<ApiResponse<List<PackageViewModel>>> GetAllSlotAsync()
        {
            return await _packageService.GetAllStandardPackageByAsync();
        }
    }
}

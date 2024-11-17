
using FranchiseProject.Application.Interfaces;

using global::FranchiseProject.Application.Commons;
using global::FranchiseProject.Application.ViewModels.EquipmentViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/equipments")]
        [ApiController]
        public class EquipmentController : ControllerBase
        {
            private readonly IEquipmentService _equipmentService;

            public EquipmentController(IEquipmentService equipmentService)
            {
                _equipmentService = equipmentService;
            }

            [SwaggerOperation(Summary = "Tạo mới thiết bị {Authorize = Manager, Admin}")]
            [HttpPost]
            public async Task<ApiResponse<bool>> CreateEquipmentAsync([FromBody] List<EquipmentRequestViewModel> models)
            {
                return await _equipmentService.CreateEquipmentAsync(models);
            }

            [SwaggerOperation(Summary = "Xuất danh sách thiết bị ra Excel {Authorize = Manager, Admin}")]
            [HttpGet("contract/{id}")]
            public async Task<ApiResponse<byte[]>> ExportEquipmentsToExcelAsync(Guid id)
            {
                return await _equipmentService.ExportEquipmentsToExcelAsync(id);
            }

            [SwaggerOperation(Summary = "Thêm thiết bị sau khi ký hợp đồng {Authorize = Manager, Admin}")]
            [HttpPost("contract/{id}")]
            public async Task<ApiResponse<bool>> AddEquipmentAfterContractSigningAsync(Guid id, [FromBody] List<EquipmentRequestViewModel> equipmentRequests)
            {
                return await _equipmentService.AddEquipmentAfterContractSigningAsync(id, equipmentRequests);
            }

            [SwaggerOperation(Summary = "Lấy danh sách thiết bị theo Agency ID {Authorize = Manager, Admin}")]
            [HttpGet("agency/{id}")]
            public async Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentsByAgencyIdAsync(Guid id, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
            {
                return await _equipmentService.GetEquipmentsByAgencyIdAsync(id, pageIndex, pageSize);
            }
        }
    }


using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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

       // [Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician)]
        [SwaggerOperation(Summary = "thêm trang thiết bị {Authorize = Manager, SystemTechnician}")]
        [HttpPost("agency/{id}")]
        public async Task<ApiResponse<object>> ImportEquipmentsFromExcelAsync(IFormFile file, Guid id)
        {
            return await _equipmentService.ImportEquipmentsFromExcelAsync(file, id);
        }
        [HttpGet("agency/{id}")]
        [SwaggerOperation(Summary = "xuất file danh sách trang bị  {Authorize = Manager, SystemTechnician}")]
       // [Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician)]
        public async Task<ApiResponse<string>> GenerateEquipmentReportAsync(Guid id)
        {
            return  await _equipmentService.GenerateEquipmentReportAsync(id);
          
        }
        [HttpPut("{id}/status")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái của thiết bị {Authorize = Manager, SystemTechnician}")]
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician)]
        public async Task<ApiResponse<bool>> UpdateEquipmentStatusAsync(Guid id, EquipmentStatusEnum equipmentStatus)
        {
            return await _equipmentService.UpdateEquipmentStatusAsync(id, equipmentStatus);
        }

        [HttpGet("`api/v1/agency/equipments")]
        [SwaggerOperation(Summary = "Lấy danh sách thiết bị theo AgencyId {Authorize = Manager, SystemTechnician, AgencyManager}")]
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        public async Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentByAgencyId( [FromQuery] FilterEquipmentViewModel filter)
        {
          
            return await _equipmentService.GetEquipmentByAgencyIdAsync(filter);
        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Cập nhật  thiết bị {Authorize = Manager, SystemTechnician}")]
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician)]
        public async Task<ApiResponse<bool>> UpdateEquipmentAsync(Guid id, UpdateEquipmentViewModel updateModel)
        {
            return await _equipmentService.UpdateEquipmentAsync(id, updateModel);
        }
        [HttpGet("`api/v1/agency/equipments/{id}")]
        [SwaggerOperation(Summary = "Lấy danh sách thiết bị theo AgencyId {Authorize = Manager, SystemTechnician, AgencyManager}")]
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemTechnician + "," + AppRole.AgencyManager)]
        public async Task<ApiResponse<List<EquipmentSerialNumberHistoryViewModel>>> GetSerialNumberHistoryByEquipmentIdAsync(Guid id)
        {

            return await _equipmentService.GetSerialNumberHistoryByEquipmentIdAsync(id);
        }
    }
}



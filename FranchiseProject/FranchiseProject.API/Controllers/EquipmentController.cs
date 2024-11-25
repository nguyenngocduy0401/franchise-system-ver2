using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
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
        public async Task<IActionResult> GenerateEquipmentReport(Guid id)
        {
            var result = await _equipmentService.GenerateEquipmentReportAsync(id);
            if (!result.isSuccess)
            {
                return BadRequest(result);
            }

            return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EquipmentReport.xlsx");
        }

    }
}



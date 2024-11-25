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

        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Import equipments from Excel file {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPost("import")]
        public async Task<ApiResponse<object>> ImportEquipmentsFromExcelAsync(IFormFile file)
        {
            return await _equipmentService.ImportEquipmentsFromExcelAsync(file);
        }

        
    }
}



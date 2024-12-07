using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Application.ViewModels.ReportViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

       // [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Tạo báo cáo khóa học {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPost("course")]
        public async Task<ApiResponse<bool>> CreateCourseReportAsync(CreateReportCourseViewModel model)
        {
            return await _reportService.CreateCourseReport(model);
        }

      //  [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Tạo báo cáo thiết bị {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPost("equipment")]
        public async Task<ApiResponse<bool>> CreateEquipmentReportAsync(CreateReportEquipmentViewModel model)
        {
            return await _reportService.CreateEquipmentReport(model);
        }

       // [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Cập nhật báo cáo khóa học {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPut("course/{id}")]
        public async Task<ApiResponse<bool>> UpdateCourseReportAsync(Guid id, UpdateReportCourseViewModel model)
        {
            return await _reportService.UpdateCourseReport(id, model);
        }

//[Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Cập nhật báo cáo thiết bị {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPut("equipment/{id}")]
        public async Task<ApiResponse<bool>> UpdateEquipmentReportAsync(Guid id, UpdateReportEquipmentViewModel model)
        {
            return await _reportService.UpdateEquipmentReport(id, model);
        }
//[Authorize]
        [SwaggerOperation(Summary = "Tìm kiếm báo cáo")]
        [HttpGet]
        public async Task<ApiResponse<Pagination<ReportViewModel>>> FilterReportAsync([FromQuery] FilterReportModel filterReportModel)
        {
            return await _reportService.FilterReportAsync(filterReportModel);
        }

//[Authorize(Roles = AppRole.Admin + ","+ AppRole.Manager +"," + AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Cập nhật trạng thái báo cáo {Authorize = Admin,Manager,AgencyStaff,AgencyManager}")]
        [HttpPut("{id}/status")]
        public async Task<ApiResponse<bool>> UpdateReportStatusAsync(Guid id, [FromBody] ReportStatusEnum newStatus)
        {
            return await _reportService.UpdateReportStatusAsync(id, newStatus);
        }

//[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "Phản hồi báo cáo {Authorize = Admin,Manager}")]
        [HttpPost("{id}/respond")]
        public async Task<ApiResponse<bool>> RespondToReportAsync(Guid id, [FromBody] string response)
        {
            return await _reportService.RespondToReportAsync(id, response);
        }

//[Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Xóa báo cáo {Authorize = AgencyStaff, AgencyManager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteReportAsync(Guid id)
        {
            return await _reportService.DeleteReportAsync(id);
        }
    }
}
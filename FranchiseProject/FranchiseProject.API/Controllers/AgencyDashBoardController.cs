using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/agency-dashboards")]
    [ApiController]
    public class AgencyDashBoardController
    {
        private IAgencyDashboardService _agencyDashboardService;
        public AgencyDashBoardController(IAgencyDashboardService agencyDashboardService)
        {
            _agencyDashboardService = agencyDashboardService;
        }
        [Authorize(Roles = AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "báo cáo theo khóa học   {Authorize = AgencyManager}")]
        [HttpGet("courses")]
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAsync(DateTime startDate, DateTime endDate)
        {
            return await _agencyDashboardService.GetCourseRevenueAsync(startDate, endDate);
        }
        [Authorize(Roles = AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "báo cáo doanh thu  theo ngày   {Authorize = AgencyManager}")]
        [HttpGet("")]
        public async Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseAsync(DateTime startDate, DateTime endDate)
        {
            return await _agencyDashboardService.GetTotalRevenueFromRegisterCourseAsync(startDate, endDate);
        }
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "báo cáo doanh thu  theo ngày by AgencyId  {Authorize = AgencyManager}")]
        [HttpGet("agencies/{id}")]
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAgencyIdAsync(Guid id, DateTime startDate, DateTime endDate)
        {
            return await _agencyDashboardService.GetCourseRevenueAgencyIdAsync(id, startDate, endDate);
        }
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "số tiền cần phải trả theo by AgencyId  {Authorize = AgencyManager}")]
        [HttpGet("agencies/{id}/amount")]
        public async Task<ApiResponse<double>> GetAmountAgencyAmountPayAsync(Guid id, DateTime startDate, DateTime endDate)
        {
            return await _agencyDashboardService.GetAmountAgencyAmountPayAsync(id, startDate, endDate);
        }
      //  [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.Manager + "," + AppRole.Admin)]
        [SwaggerOperation(Summary = "truy xuất doanh thu theo năm")]
        [HttpGet("financial-report/{agencyId}/{year}")]
        public async Task<ApiResponse<List<AgencyFinancialReport>>> GetAgencyFinancialReportAsync(Guid agencyId, int year)
        => await _agencyDashboardService.GetAgencyFinancialReportAsync(agencyId, year);

        [SwaggerOperation(Summary = "truy xuất file excel doanh thu theo năm")]
        [HttpGet("financial-report/{agencyId}/{year}/excel")]
        public async Task<ApiResponse<string>> GetFileExcelAgencyFinancialReportAsync(Guid agencyId, int year)
    => await _agencyDashboardService.GetFileExcelAgencyFinancialReportAsync(agencyId, year);
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "Tạo báo cáo tài chính hàng tháng cho đại lý theo tháng  {Authorize = AgencyManager}")]
        [HttpGet("monthly-financial-report")]
        public async Task<ApiResponse<string>> GetAgencyMonthlyFinancialReport([FromQuery] Guid agencyId, [FromQuery] int month, [FromQuery] int year)
        {
            return await _agencyDashboardService.GetFileExcelAgencyMonthlyFinancialReportAsync(agencyId, month, year);
        }
    }
}

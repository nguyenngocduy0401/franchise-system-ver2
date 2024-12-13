using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.DashBoard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
      //  [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "Phân tích doanh thu hàng tháng cho một năm cụ thể")]
        [HttpGet("monthly-revenue")]
        public async Task<ApiResponse<List<MonthlyRevenueViewModel>>> AnalyzeMonthlyRevenueAsync(int year)
        {
            return await _dashboardService.AnalyzeMonthlyRevenueAsync(year);
        }
        //[Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "Lấy thống kê đại lý trong khoảng thời gian")]
        [HttpGet("agency-statistics")]
        public async Task<ApiResponse<Pagination<AgencyStatisticsViewModel>>> GetAgencyStatistics(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {
            return await _dashboardService.GetAgencyStatistics(startDate, endDate, pageNumber, pageSize);
        }
        //[Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "Lấy thống kê doanh thu tổng thể")]
        [HttpGet("revenue-statistics")]
        public async Task<ApiResponse<RevenueStatisticsViewModel>> GetRevenueStatistics()
        {
            return await _dashboardService.GetRevenueStatistics();
        }
        [HttpGet("revenue-share-percentage")]
        [SwaggerOperation(Summary = "Phần trăm doanh thu chia sẻ từ từng đối tác nhượng quyền (payment có Type monthly)")]
        public async Task<ApiResponse<List<PartnerRevenueShare>>> GetRevenueSharePercentageAsync()
        {
            return await _dashboardService.CalculateRevenueSharePercentageAsync();
        }
    }
}

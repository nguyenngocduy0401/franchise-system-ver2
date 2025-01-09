using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.DashBoard;
using FranchiseProject.Application.ViewModels.DashBoardViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<AdminDashboardModel>> AdminCourseRevenueDashboardAsync(DateOnly from, DateOnly to, bool year, int topCourse);
        Task<ApiResponse<RevenueStatisticsViewModel>> GetRevenueStatistics();
        Task<ApiResponse<Pagination<AgencyStatisticsViewModel>>> GetAgencyStatistics(DateTime startDate, DateTime endDate, int pageNumber, int pageSize);
        Task<ApiResponse<List<MonthlyRevenueViewModel>>> AnalyzeMonthlyRevenueAsync(int year);
        Task<ApiResponse<List<PartnerRevenueShare>>> CalculateRevenueSharePercentageAsync();
        Task<ApiResponse<string>> GenerateAgencyPaymentReportAsync(int month, int year);
    }
}

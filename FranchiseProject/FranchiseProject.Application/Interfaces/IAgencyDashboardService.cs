using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAgencyDashboardService
    {
        Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAgencyIdAsync(Guid AgencyId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<double>> GetAmountAgencyAmountPayAsync(Guid AgencyId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<AgencyFinancialReport>>> GetAgencyFinancialReportAsync(Guid agencyId, int year);
        Task<ApiResponse<string>> GetFileExcelAgencyFinancialReportAsync(Guid agencyId, int year);
        Task<ApiResponse<string>> GetFileExcelAgencyMonthlyFinancialReportAsync(Guid agencyId, int month, int year);
    }
}

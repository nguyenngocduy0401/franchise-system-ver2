using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
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
    }
}

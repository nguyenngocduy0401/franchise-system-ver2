using FranchiseProject.Application.ViewModels.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IRevenueRepository
    {
        Task<List<RevenueData>> GetRevenueByMonthAsync(int year);
        Task<List<PartnerRevenueShare>> GetMonthlyRevenueShareAsync();
    }
}

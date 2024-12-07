using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DashBoard
{
    public class RevenueStatisticsViewModel
    {
        public double TotalRevenue { get; set; }
        public double CollectedRevenue { get; set; }
        public double UnpaidRevenue { get; set; }
    }
}

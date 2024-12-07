using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DashBoard
{
    public class AgencyStatisticsViewModel
    {
        public Guid AgencyId { get; set; }
        public string AgencyName { get; set; }
        public int TotalStudents { get; set; }
        public double TotalRevenue { get; set; }
        public double RevenueToHeadquarters { get; set; }
        public string MonthlyPaymentStatus { get; set; }
    }
}

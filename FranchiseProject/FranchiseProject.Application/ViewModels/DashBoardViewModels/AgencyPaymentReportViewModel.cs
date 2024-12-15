using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DashBoardViewModels
{
    public class AgencyPaymentReportViewModel
    {
        public int RowNumber { get; set; }
        public string AgencyName { get; set; }
        public string MonthYear { get; set; }
        public decimal FranchiseFee { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal RevenuePercentage { get; set; }
        public decimal SharedAmount { get; set; }
    }
}

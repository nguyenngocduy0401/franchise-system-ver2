using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DashBoard
{
    public class PartnerRevenueShare
    {
        public Guid AgencyId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RevenuePercentage { get; set; }
    }
}

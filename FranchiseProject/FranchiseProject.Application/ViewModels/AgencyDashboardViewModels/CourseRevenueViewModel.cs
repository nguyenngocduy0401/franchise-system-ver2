using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AgencyDashboardViewModels
{
    public class CourseRevenueViewModel
    {
        public Guid CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public int StudentCount { get; set; }
        public double? TotalRevenue { get; set; }
        public double? MonthlyFee { get; set; } // tiền mà phải trả chia sẻ doanh thu 
        public double? Refunds { get; set; }//Hoàn tiền
       public double? ActualProfits { get; set; }//Lợi nhuận thực tế là doanh thu trừ đi tiền đã hoàn và trừ đi số tiền chia sẻ doanh thu hằng tháng
    }
}

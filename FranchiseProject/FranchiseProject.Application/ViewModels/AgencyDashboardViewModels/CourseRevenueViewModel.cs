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
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int StudentCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

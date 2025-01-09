using FranchiseProject.Application.ViewModels.CourseViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DashBoardViewModels
{
    public class AdminDashboardModel
    {
        public List<AdminCourseRevenueDashboard>? adminCourseRevenueDashboards { get; set; }
        public double? totalCourseRevenue { get; set; }
        public List<AdminContractRevenueDashboard>? adminContractRevenueDashboard { get; set; }
        public double? totalContractRevenue { get; set; }
        public List<CourseDashboardModel>? courseDashboardModels { get; set; }
    }
}

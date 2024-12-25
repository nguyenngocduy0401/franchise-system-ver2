using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.VnPayViewModels
{
    public class AgencyCoursePaymentViewModel
    {
        public Guid AgencyId { get; set; }
        public Guid CourseId { get; set; }
        public string? UserId { get; set; }
        public Guid RegisterCourseId { get; set; }
        
    }
}

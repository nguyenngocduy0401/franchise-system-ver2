using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.VnPayViewModels
{
    public class AgencyCoursePaymentViewModel
    {
        public string? Gmail { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid AgencyId { get; set; }
        public Guid CourseId { get; set; }
    }
}

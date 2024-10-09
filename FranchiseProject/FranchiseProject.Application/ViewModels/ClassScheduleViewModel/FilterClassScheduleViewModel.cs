using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassScheduleViewModel
{
    public class FilterClassScheduleViewModel
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
     
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

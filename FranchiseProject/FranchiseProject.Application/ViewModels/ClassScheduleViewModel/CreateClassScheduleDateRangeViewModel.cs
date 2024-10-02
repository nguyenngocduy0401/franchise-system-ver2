using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassScheduleViewModel
{
    public class CreateClassScheduleDateRangeViewModel
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Room { get; set; }
        public string? ClassId { get; set; }
        public string? SlotId { get; set; }
    }
}

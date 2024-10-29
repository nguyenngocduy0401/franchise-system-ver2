using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModels
{
    public  class StudentScheduleViewModel
    {
        public Guid ScheduleId { get; set; }
        public Guid ClassId { get; set; }
        public Guid SlotId { get; set; }    
        public string? Room { get; set; }
        public string? ClassName { get; set; }
        public string ? SlotName {  get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
     
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassScheduleViewModels
{
    public class CreateClassScheduleViewModel
    { 
        public string? Date { get; set; }
        public string? Room {  get; set; }
        public string? ClassId {  get; set; }
        public string? SlotId { get; set; }
        public string? Url { get; set; }

    }
}

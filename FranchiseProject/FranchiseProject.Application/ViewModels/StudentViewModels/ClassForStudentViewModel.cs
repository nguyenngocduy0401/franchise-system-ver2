using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModels
{
    public class ClassForStudentViewModel
    {
        public Guid? ClassId { get; set; }
        public string? ClassName {  get; set; }
        public string? CourseName {  get; set; }
        public string? SlotName {  get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public List<DayOfWeekEnum>? dayOfWeek {  get; set; } 

    }
}

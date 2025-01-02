using FranchiseProject.Application.ViewModels.AttendanceViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModels
{
    public class ClassScheduleByInstructorViewModel
    {

        public int Total { get; set; }
        public int Future { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public string? ClassName { get; set; }

        public List<AttendanceClassByInstructorViewModel>? ClassSchedules { get; set; }
    }
}

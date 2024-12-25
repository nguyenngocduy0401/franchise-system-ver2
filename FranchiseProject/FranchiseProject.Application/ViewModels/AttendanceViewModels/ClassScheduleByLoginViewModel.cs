using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AttendanceViewModels
{
    public class ClassScheduleByLoginViewModel
    {
        public int Total { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Future { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public string? ClassName { get; set; }
        public List<AttendanceClassByLoginViewModel>? ClassSchedules{ get; set; }
    }
}

using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModel
{
    public class ClassStudentViewModel
    {
        public Guid? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? DayOfWeek { get; set; }
        public string? InstructorName { get; set; }
        public List<StudentClassViewModel>? StudentInfo { get; set; }
        public SlotViewModel? SlotViewModels { get; set; }
    }
}

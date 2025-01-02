using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModel
{
    public class ClassViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public string? InstructorName { get; set; }
        public string? CourseName { get; set; }
        public string? DayOfWeek { get; set; }
        public string? SlotStart { get; set; }
        public string? SlotEnd { get; set; } 
        public string? StartDate { get; set; }
        public int? DaysElapsed { get; set; }
        public ClassStatusEnum Status { get; set; }
        public Guid? CourseId { get; set; }
        public int? TotalLessons { get; set; }
    }
}

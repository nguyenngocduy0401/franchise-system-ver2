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
        public string? ClassName {  get; set; }
        public string? Capicity {  get; set; }
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public string? Name { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public List<StudentClassViewModel>? StudentInfo { get; set; }

    }
}

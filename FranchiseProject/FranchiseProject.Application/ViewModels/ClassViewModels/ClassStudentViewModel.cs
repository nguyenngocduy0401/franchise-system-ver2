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
  
        public List<StudentClassViewModel>? StudentInfo { get; set; }

    }
}

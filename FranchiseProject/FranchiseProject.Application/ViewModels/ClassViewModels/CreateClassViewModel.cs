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
    public class CreateClassViewModel
    {
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? CourseId { get; set; }
        public string? InstructorId { get; set; }
         public List<string >? StudentId { get; set; }
     //   public ClassStatusEnum? Status { get; set; }
    }
}

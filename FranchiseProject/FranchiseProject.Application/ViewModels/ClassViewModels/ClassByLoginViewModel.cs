using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModels
{
    public class ClassByLoginViewModel
    {
        public Guid? ClassId { get; set; }
        public string? ClassName { get; set; }
        public Guid? CourseId { get; set; }
         public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

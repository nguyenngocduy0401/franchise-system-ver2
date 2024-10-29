using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModels
{
    public class UpdateClassViewModel
    {
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? InstructorId {  get; set; }
    }
}

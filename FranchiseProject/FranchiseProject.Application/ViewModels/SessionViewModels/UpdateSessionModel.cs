using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.SessionViewModels
{
    public class UpdateSessionModel
    {
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Chapter { get; set; }
        public string? Description { get; set; }
    }
}

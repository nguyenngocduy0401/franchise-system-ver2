using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.MaterialViewModels
{
    public class CreateMaterialModel
    {
        public string? URL { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }
    }
}

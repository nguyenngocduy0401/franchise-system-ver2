using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseMaterialViewModels
{
    public class CourseMaterialViewModel
    {
        public Guid? Id { get; set; }
        public string? URL { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }
    }
}

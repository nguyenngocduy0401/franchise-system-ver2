using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class CourseViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? URLImage { get; set; }
        public string? NumberOfSession { get; set; }
        public CourseStatusEnum Status { get; set; }
        public Guid? SyllabusId { get; set; }
        public Guid? CourseCategoryId { get; set; }
    }
}

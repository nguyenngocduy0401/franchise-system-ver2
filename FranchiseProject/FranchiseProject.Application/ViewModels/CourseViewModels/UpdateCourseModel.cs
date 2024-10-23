using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class UpdateCourseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? URLImage { get; set; }
        public int NumberOfLession { get; set; }
        public int Price { get; set; }
        public Guid? CourseCategoryId { get; set; }
    }
}

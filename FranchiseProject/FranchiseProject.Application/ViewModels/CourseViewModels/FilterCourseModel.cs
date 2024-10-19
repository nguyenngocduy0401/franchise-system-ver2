using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class FilterCourseModel
    {
        public string? Search {get; set;}
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public CourseStatusEnum? Status { get; set; }
        public Guid? CourseCategoryId { get; set; }
        public SortCourseStatusEnum? SortBy { get; set; }
        public SortDirectionEnum? SortDirection { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

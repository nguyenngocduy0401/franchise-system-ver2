using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.RegisterCourseViewModels
{
    public class FilterRegisterCourseViewModel
    {
        public Guid? CourseId { get; set; }
        public StudentCourseStatusEnum? StudentCourseStatus { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

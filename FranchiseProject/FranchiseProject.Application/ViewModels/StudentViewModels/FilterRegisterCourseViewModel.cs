using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModel
{
    public class FilterRegisterCourseViewModel
    {
        public StudentCourseStatusEnum? Status {  get; set; }
        public StudentPaymentStatusEnum? PaymentStatus { get; set; }
        public string? CourseId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

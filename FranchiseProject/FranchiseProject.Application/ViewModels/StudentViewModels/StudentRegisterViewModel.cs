using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModels
{
    public class StudentRegisterViewModel
    {
        public Guid?   Id { get; set; }
        public string? UserId { get; set; }
        public Guid? CourseId { get; set; }
        public string? FullName { get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber {  get; set; }
        public string? CourseCode { get; set; }
        public int? CoursePrice { get; set; }
        public string? RegisterDate { get; set; }
        public DateTime? PaymentDeadline  { get; set; }
        public StudentCourseStatusEnum? StudentStatus { get; set; }
        public StudentPaymentStatusEnum? PaymentStatus { get; set; }
        public string? DateTime { get; set; }
    }
}

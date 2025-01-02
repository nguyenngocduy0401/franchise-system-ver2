using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public DateTime? CreationDate { get; set; }
        public string? ModificationDate { get; set; } 
        public string? ConsultantName { get; set; }
        public DateTime? PaymentDeadline  { get; set; }
        public StudentCourseStatusEnum? StudentStatus { get; set; }
        public StudentPaymentStatusEnum? PaymentStatus { get; set; }
        public double? StudentAmountPaid { get; set; } 
        public string? DateTime { get; set; }
        public string? ClassSchedule { get; set; } // lấy từ DayOfWeek trong class và Start time , end time của Slot thuộc classSchedule của lớp đó 
        public DateOnly? StartDate { get; set; }// ngày bắt đầu khóa học (classSchedule)
        public DateOnly? EndDate { get; set; }// ngày kết thúc khóa học (classSchedule)
        public DateTime? PaymentDate { get; set; }
        public string ? ClassName { get; set; }
    }
}

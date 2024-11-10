using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class RegisterCourse : BaseEntity
    {
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public string? DateTime {  get; set; }
        public string? Email { get; set; }
        public DateTime? PaymentDeadline { get; set; }
        public StudentCourseStatusEnum? StudentCourseStatus { get; set; }
        public StudentPaymentStatusEnum? StudentPaymentStatus { get; set; }
        public DateTime? CreatDate { get; set; }
    }
}

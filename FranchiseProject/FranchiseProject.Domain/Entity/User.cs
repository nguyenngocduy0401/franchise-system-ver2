using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? URLImage { get; set; }
        public string? OTPEmail { get; set; }
        public DateTime? ExpireOTPEmail { get; set; }
        public DateTime? Expire { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
        public UserStatusEnum? Status { get; set; }
        public StudentStatusEnum? StudentStatus { get; set; }           
        public StudentPaymentStatusEnum? StudentPaymentStatus { get; set; }
        public virtual ICollection<Score>? Scores { get; set; } 
        public virtual ICollection<Report>? Reports { get; set; }
        public virtual ICollection<RegisterCourse>? RegisterCourses { get; set; }
        public virtual ICollection<ClassRoom>? ClassRooms { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<AssignmentSubmit>? AssignmentSubmits { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }
        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
        public virtual ICollection<UserAppointment>? UserAppointments { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}

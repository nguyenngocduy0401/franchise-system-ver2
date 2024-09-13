using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class StudentCourse
    {
        public StudentCourseStatusEnum Status { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

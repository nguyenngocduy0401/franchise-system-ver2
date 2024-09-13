using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Class : BaseEntity
    {
        public int Capacity { get; set; }
        public int CurrentEnrollment { get; set; }
        public string? Name { get; set; }
        public Guid? TermId { get; set; }
        [ForeignKey("TermId")]
        public Term? Term { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public virtual ICollection<StudentClass>? StudentClasses { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; }
        public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
    }
}

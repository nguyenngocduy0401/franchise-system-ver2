using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Assessment : BaseEntity
    {
        public int Number { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public string? Content { get; set; }
        public int Quantity { get; set; }
        public double? Weight { get; set; }
        public double? CompletionCriteria { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; }
        public virtual ICollection<Quiz>? Quizzes { get; set; }
    }
}


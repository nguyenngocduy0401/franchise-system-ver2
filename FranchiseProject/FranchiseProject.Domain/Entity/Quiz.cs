using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Quiz : BaseEntity
    {
        public int Quantity { get; set; }
        public int? Duration { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public QuizTypeEnum Type { get; set; }
        public DateTime? StartTime { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
        public Guid? AssessmentId { get; set; }
        [ForeignKey("AssessmentId")]
        public Assessment? Assessment { get; set; }
        public virtual ICollection<QuizDetail>? QuizDetails { get; set; }
        public virtual ICollection<Score>? Scores { get; set; }
    }
}

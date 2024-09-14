using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class FeedbackQuestion : BaseEntity
    {
        public string? Description { get; set; }
        public QuestionTypeEnum QuestionType { get; set; }
        public Guid? FeedbackId { get; set; }
        [ForeignKey("FeedbackId")]
        public Feedback? Feedback { get; set; }
        public virtual ICollection<FeedbackOption>? FeedbackQuestions { get; set; }
    }
}

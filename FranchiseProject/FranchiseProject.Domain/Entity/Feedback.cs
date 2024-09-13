using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Feedback : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
        public virtual ICollection<FeedbackQuestion>? QuestionFeedbacks { get; set; }
        public virtual ICollection<FeedbackAnswer>? FeedbackAnswers { get; set; }
    }
}

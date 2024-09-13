using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class FeedbackOption : BaseEntity
    {
        public string? OptionText { get; set; }
        public Guid? FeedbackQuestionId { get; set; }
        [ForeignKey("FeedbackQuestionId")]
        public FeedbackQuestion? FeedbackQuestion { get; set; }
        public virtual ICollection<FeedbackAnswer>? FeedbackAnswers { get; set; }
    }
}

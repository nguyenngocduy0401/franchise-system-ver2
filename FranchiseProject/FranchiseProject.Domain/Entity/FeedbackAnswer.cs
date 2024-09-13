using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class FeedbackAnswer : BaseEntity
    {
        public string? Comment { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? FeedbackOptionId { get; set; }
        [ForeignKey("FeedbackOptionId")]
        public FeedbackOption? FeedbackOption { get; set; }
        public Guid? FeedbackId { get; set; }
        [ForeignKey("FeedbackId")]
        public Feedback? Feedback { get; set; }
    }
}

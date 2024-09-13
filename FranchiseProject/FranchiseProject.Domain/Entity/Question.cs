using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Question : BaseEntity
    {
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public double? Score { get; set; }
        public Guid? ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
        public virtual ICollection<QuizDetail>? QuizDetails { get; set; }
        public virtual ICollection<QuestionOption>? QuestionOptions { get; set; }
    }
}

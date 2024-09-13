using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class QuestionOption : BaseEntity
    {
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public QuestionOptionStatusEnum Status { get; set; }
        public Guid? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
    }
}

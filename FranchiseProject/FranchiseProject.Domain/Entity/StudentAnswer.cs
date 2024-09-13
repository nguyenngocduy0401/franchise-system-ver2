using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class StudentAnswer
    {
        public Guid? QuestionOptionId { get; set; }
        [ForeignKey("QuestionOptionId")]
        public QuestionOption? QuestionOptions { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }  
    }
}

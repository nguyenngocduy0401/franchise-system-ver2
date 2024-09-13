using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class QuizDetail
    {
        public Guid? QuizId { get; set; }
        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }
        public Guid? QuestionId { get; set; }
        [ForeignKey("QuizTestId")]
        public Question? Question { get; set; }
    }
}

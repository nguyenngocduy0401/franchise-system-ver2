using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Score : BaseEntity
    {
        public double ScoreNumber { get; set; }
        public Guid? QuizId { get; set; }
        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

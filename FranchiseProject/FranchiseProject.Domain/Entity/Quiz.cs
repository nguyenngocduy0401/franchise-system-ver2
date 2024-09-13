using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Quiz : BaseEntity
    {
        public double? Duration { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public virtual ICollection<QuizDetail>? QuizDetails { get; set; }
        public virtual ICollection<Score>? Scores { get; set; }
    }
}

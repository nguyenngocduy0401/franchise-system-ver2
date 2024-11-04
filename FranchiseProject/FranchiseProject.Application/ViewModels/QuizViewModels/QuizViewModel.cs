using FranchiseProject.Application.ViewModels.ScoreViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuizViewModels
{
    public class QuizViewModel
    {
        public Guid? Id { get; set; }
        public int Quantity { get; set; }
        public int? Duration { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public Guid? ClassId { get; set; }
        public ICollection<UserScoreViewModel>? UserScores { get; set; }
    }
}

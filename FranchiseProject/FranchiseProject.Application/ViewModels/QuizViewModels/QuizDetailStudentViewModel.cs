using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuizViewModels
{
    public class QuizDetailStudentViewModel
    {
        public Guid? Id { get; set; }
        public int Quantity { get; set; }
        public int? Duration { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public Guid? ClassId { get; set; }
        public virtual ICollection<QuestionStudentViewModel>? Questions { get; set; }
    }
}

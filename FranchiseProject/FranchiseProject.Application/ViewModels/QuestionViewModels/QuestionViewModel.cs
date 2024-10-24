using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuestionViewModels
{
    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public Guid? ChapterId { get; set; }
        public ICollection<QuestionOptionViewModel>? QuestionOptions { get; set; }
    }
}

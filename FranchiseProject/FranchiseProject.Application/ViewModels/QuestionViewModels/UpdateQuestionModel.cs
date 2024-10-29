using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuestionViewModels
{
    public class UpdateQuestionModel
    {
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public List<CreateQuestionOptionArrangeModel>? QuestionOptions { get; set; }
    }
}

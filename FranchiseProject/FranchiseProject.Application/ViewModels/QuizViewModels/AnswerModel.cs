using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuizViewModels
{
    public class AnswerModel
    {
        public ICollection<Guid>? QuestionOptionsId { get; set; }

    }
}

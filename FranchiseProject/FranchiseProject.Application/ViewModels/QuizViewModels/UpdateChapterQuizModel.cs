using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.QuizViewModels
{
    public class UpdateChapterQuizModel
    {
        public int Quantity { get; set; }
        public List<Guid> ChapterIds { get; set; }

    }
}

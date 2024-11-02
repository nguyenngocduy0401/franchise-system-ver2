using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IQuizService
    {
        Task<ApiResponse<bool>> CreateQuizForClass(CreateQuizModel createQuizModel);
        Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizForStudentByQuizId(Guid id);
    }
}

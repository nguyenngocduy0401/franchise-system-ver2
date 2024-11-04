using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IQuizService
    {

        Task<ApiResponse<bool>> UpdateQuizByIdAsync(Guid quizId, UpdateQuizModel updateQuizModel);
        Task<ApiResponse<bool>> DeleteQuizByIdAsync(Guid quizId);
        Task<ApiResponse<bool>> SubmitQuiz(Guid quizId, AnswerModel answerModel);
        Task<ApiResponse<IEnumerable<QuizViewModel>>> GetAllQuizByClassId(Guid id);
        Task<ApiResponse<bool>> CreateQuizForClass(CreateQuizModel createQuizModel);
        Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizDetailForStudentByQuizId(Guid id);
        Task<ApiResponse<IEnumerable<QuizStudentViewModel>>> GetAllQuizForStudentByClassId(Guid id);
    }
}

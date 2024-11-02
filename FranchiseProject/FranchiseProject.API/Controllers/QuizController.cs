using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }
        [SwaggerOperation(Summary = "tạo bài kiểm tra {Authorize = Instructor, Manager}")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> CreateQuestionByIdAsync(CreateQuizModel createQuizModel)
        {
            return await _quizService.CreateQuizForClass(createQuizModel);
        }
        [SwaggerOperation(Summary = "tạo bài kiểm tra {Authorize = Instructor, Manager}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizForStudentByQuizId(Guid id)
        {
            return await _quizService.GetQuizForStudentByQuizId(id);
        }
    }
}

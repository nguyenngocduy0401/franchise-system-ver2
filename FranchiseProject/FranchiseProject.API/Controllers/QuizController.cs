﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Application.ViewModels.QuizViewModels;
using Microsoft.AspNetCore.Authorization;
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
        [SwaggerOperation(Summary = "học sinh lấy bài kiểm tra {Authorize = Instructor, Manager}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<QuizDetailStudentViewModel>> GetQuizForStudentByQuizId(Guid id)
        {
            return await _quizService.GetQuizDetailForStudentByQuizId(id);
        }
        [Authorize(Roles = AppRole.Student)]
        [SwaggerOperation(Summary = "học sinh nộp bài kiểm tra {Authorize = Student}")]
        [HttpPost("~/student/api/v1/quizzes/{id}")]
        public async Task<ApiResponse<bool>> SubmitQuiz(Guid id, AnswerModel answerModel)
        {
            return await _quizService.SubmitQuiz(id, answerModel);
        }
        [Authorize(Roles = AppRole.Instructor)]
        [SwaggerOperation(Summary = "tạo bài kiểm tra {Authorize = Instructor}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateQuizByIdAsync(Guid id, UpdateQuizModel updateQuizModel)
        {
            return await _quizService.UpdateQuizByIdAsync(id, updateQuizModel);
        }
        [Authorize(Roles = AppRole.Instructor)]
        [SwaggerOperation(Summary = "xóa bài kiểm tra {Authorize = Instructor}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteQuizByIdAsync(Guid quizId)
        {
            return await _quizService.DeleteQuizByIdAsync(quizId);
        }
    }
}
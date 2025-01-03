﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IQuestionService
    {
        Task<ApiResponse<bool>> CreateQuestionArrangeAsync(Guid chapterId, List<CreateQuestionArrangeModel> createQuestionArrangeModel);
        Task<ApiResponse<List<QuestionViewModel>>> GetAllQuestionByChapterId(Guid chapterId);
        Task<ApiResponse<bool>> CreateQuestionByChapterIdAsync(Guid chapterId, CreateQuestionArrangeModel createQuestionArrangeModel);
        Task<ApiResponse<bool>> DeleteQuestionByIdAsync(Guid questionId);
        Task<ApiResponse<bool>> UpdateQuestionByIdAsync(Guid id, UpdateQuestionModel updateQuestionModel);
        Task<ApiResponse<bool>> CreateQuestionByFileAsync(Guid id, IFormFile file);
    }
}

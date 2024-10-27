using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/questions")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }
        //[Authorize(Roles = AppRole.Instructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa câu hỏi của chương học bằng id {Authorize = Instructor, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteQuestionByIdAsync(Guid id)
        {
            return await _questionService.DeleteQuestionByIdAsync(id);
        }

        //[Authorize(Roles = AppRole.Instructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật câu hỏi của chương học bằng id {Authorize = Instructor, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateQuestionIdAsync(Guid id, UpdateQuestionModel updateQuestionModel)
        {
            return await _questionService.UpdateQuestionByIdAsync(id, updateQuestionModel);
        }
        
    }
}

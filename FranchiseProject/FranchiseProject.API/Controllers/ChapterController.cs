using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/chapters")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly IQuestionService _questionService;
        public ChapterController(IChapterService chapterService, IQuestionService questionService)
        {
            _chapterService = chapterService;
            _questionService = questionService;
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo câu hỏi của chương học {Authorize = Instructor, Manager}")]
        [HttpPost("{id}/questions")]
        public async Task<ApiResponse<bool>> CreateQuestionByChapterIdAsync(Guid id, CreateQuestionArrangeModel createQuestionArrangeModel)
        {
            return await _questionService.CreateQuestionByChapterIdAsync(id, createQuestionArrangeModel);
        }
        [SwaggerOperation(Summary = "lấy tất cả câu hỏi của chương học bằng id {Authorize = Instructor, Manager}")]
        [HttpGet("{id}/questions")]
        public async Task<ApiResponse<List<QuestionViewModel>>> GetAllQuestionByChapterIdAsync(Guid id)
        {
            return await _questionService.GetAllQuestionByChapterId(id);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa chương của khóa học bằng id {Authorize = Instructor, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteChapterByIdAsync(Guid id)
        {
            return await _chapterService.DeleteChapterByIdAsync(id);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo chương của khóa học {Authorize = Instructor, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateChapterAsync(CreateChapterModel createChapterModel)
        {
            return await _chapterService.CreateChapterAsync(createChapterModel);
        }
        [Authorize(Roles = AppRole.SystemInstructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật chương của khóa học {Authorize = Instructor, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateChapterAsync(Guid id, UpdateChapterModel updateChapterModel)
        {
            return await _chapterService.UpdateChapterAsync(id, updateChapterModel);
        }
        [SwaggerOperation(Summary = "tìm kiếm chương của khóa học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<ChapterDetailViewModel>> GetChapterByIdAsync(Guid id)
        {
            return await _chapterService.GetChapterByIdAsync(id);
        }
    }
}

using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/chapter-materials")]
    [ApiController]
    public class ChapterMaterialController : ControllerBase
    {
        private readonly IChapterMaterialService _chapterMaterialService;
        public ChapterMaterialController(IChapterMaterialService chapterMaterialService)
        {
            _chapterMaterialService = chapterMaterialService;
        }

        //[Authorize(Roles = AppRole.Instructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa tài nguyên của chương học bằng id {Authorize = Instructor, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteChapterMaterialByIdAsync(Guid id)
        {
            return await _chapterMaterialService.DeleteChapterMaterialByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Instructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo tài nguyên của chương học {Authorize = Instructor, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateChapterMaterialAsync(CreateChapterMaterialModel createChapterMaterialModel)
        {
            return await _chapterMaterialService.CreateChapterMaterialAsync(createChapterMaterialModel);
        }
        //[Authorize(Roles = AppRole.Instructor + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật tài nguyên của chương học bằng id {Authorize = Instructor, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateChapterMaterialByIdAsync(Guid id, UpdateChapterMaterialModel updateChapterMaterialModel)
        {
            return await _chapterMaterialService.UpdateChapterMaterialAsync(id, updateChapterMaterialModel);
        }
        
    }
}

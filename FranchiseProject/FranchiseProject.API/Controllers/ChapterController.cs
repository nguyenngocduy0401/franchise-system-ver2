/*using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
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
        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa chương của khóa học bằng id {Authorize = Admin, Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteChapterByIdAsync(Guid id)
        {
            return await _chapterService.DeleteChapterByIdAsync(id);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo chương của khóa học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateChapterAsync(CreateChapterModel createChapterModel)
        {
            return await _chapterService.CreateChapterAsync(createChapterModel);
        }
        //[Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật chương của khóa học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateChapterAsync(Guid id, UpdateChapterModel updateChapterModel)
        {
            return await _chapterService.UpdateChapterAsync(id, updateChapterModel);
        }
        [SwaggerOperation(Summary = "tìm kiếm chương của khóa học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<ChapterViewModel>> GetChapterByIdAsync(Guid id)
        {
            return await _chapterService.GetChapterByIdAsync(id);
        }
    }
}
*/
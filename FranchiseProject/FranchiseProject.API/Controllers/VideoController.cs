using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FranchiseProject.Application.Interfaces;

namespace FranchiseProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVimeoService _vimeoService;

        public VideoController(IVimeoService vimeoService)
        {
            _vimeoService = vimeoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo(IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest("File video không hợp lệ.");

            var result = await _vimeoService.UploadVideo(videoFile);

            if (result.isSuccess)
                return Ok(new { message = "Tải video thành công!", videoUrl = result.Data });

            return StatusCode(500, new { message = result.Message });
        }
    }
}
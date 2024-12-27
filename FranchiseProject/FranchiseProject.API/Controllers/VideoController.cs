using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.VideoViewModels;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "upload video {Authorize = SystemInstructor, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<VideoViewModel>> UploadVideo(string name, IFormFile videoFile)
        {
            return await _videoService.UploadVideoAsync(videoFile, name);
        }
        //[Authorize(Roles = AppRole.Manager + "," + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "upload video {Authorize = SystemInstructor, Manager}")]
        [HttpGet()]
        public async Task<ApiResponse<List<VideoViewModel>>> GetUploadVideo()
        {
            return await _videoService.GetAllVideoAsync();
        }
        [SwaggerOperation(Summary = "delete video {Authorize = SystemInstructor, Manager}")]
        [HttpDelete()]
        public async Task<ApiResponse<bool>> DeleteVideo(Guid id)
        {
            return await _videoService.DeleteVideoAsync(id);
        }
    }
}
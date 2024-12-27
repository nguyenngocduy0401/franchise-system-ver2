using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.VideoViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IVideoService
    {
        Task<ApiResponse<VideoViewModel>> UploadVideoAsync(IFormFile videoFile, string name);
        Task<ApiResponse<bool>> DeleteVideoAsync(Guid id);
        Task<ApiResponse<List<VideoViewModel>>> GetAllVideoAsync();
    }
}

using FranchiseProject.Application.Commons;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IVimeoService
    {
        Task<ApiResponse<string>> UploadVideo(IFormFile videoFile);
    }
}

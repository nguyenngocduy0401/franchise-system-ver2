using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IChapterService
    {
        Task<ApiResponse<bool>> DeleteChapterByIdAsync(Guid chapterId);
        Task<ApiResponse<ChapterViewModel>> GetChapterByIdAsync(Guid chapterId);
        Task<ApiResponse<bool>> UpdateChapterAsync(Guid chapterId, UpdateChapterModel updateChapterModel);
        Task<ApiResponse<bool>> CreateChapterAsync(CreateChapterModel createChapterModel);
    }
}

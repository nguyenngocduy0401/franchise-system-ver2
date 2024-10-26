using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IChapterMaterialService
    {
        Task<ApiResponse<bool>> DeleteChapterMaterialByIdAsync(Guid chapterMaterialId);
        Task<ApiResponse<bool>> UpdateChapterMaterialAsync(Guid chapterMaterialId, UpdateChapterMaterialModel updateChapterMaterialModel);
        Task<ApiResponse<bool>> CreateChapterMaterialAsync(CreateChapterMaterialModel createChapterMaterialModel);
        
    }
}

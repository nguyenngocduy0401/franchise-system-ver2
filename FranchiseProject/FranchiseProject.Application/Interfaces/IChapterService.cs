using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IChapterService
    {
        Task<ApiResponse<List<ChapterViewModel>>> GetChapterByClassIdAsync(Guid classId);
        Task<ApiResponse<List<ChapterViewModel>>> GetChapterByCourseIdAsync(Guid courseId);
        Task<ApiResponse<bool>> DeleteChapterByIdAsync(Guid chapterId);
        Task<ApiResponse<ChapterDetailViewModel>> GetChapterByIdAsync(Guid chapterId);
        Task<ApiResponse<bool>> UpdateChapterAsync(Guid chapterId, UpdateChapterModel updateChapterModel);
        Task<ApiResponse<bool>> CreateChapterAsync(CreateChapterModel createChapterModel);
        Task<ApiResponse<bool>> CreateChapterArrangeAsync(Guid courseId, List<CreateChapterArrangeModel> createChapterArrangeModel);
    }
}

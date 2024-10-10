using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ICourseCategoryService
    {
        Task<ApiResponse<List<CourseCategoryViewModel>>> GetAllCourseCategoryAsync();
        Task<ApiResponse<Pagination<CourseCategoryViewModel>>> FilterCourseCategoryAsync(FilterCourseCategoryModel filterCourseCategoryModel);
        Task<ApiResponse<bool>> DeleteCourseCategoryByIdAsync(Guid courseCategoryId);
        Task<ApiResponse<CourseCategoryViewModel>> GetCourseCategoryByIdAsync(Guid courseCategoryId);
        Task<ApiResponse<bool>> UpdateCourseCategoryAsync(Guid courseCategoryId, UpdateCourseCategoryModel updateCourseCategoryModel);
        Task<ApiResponse<bool>> CreateCourseCategoryAsync(CreateCourseCategoryModel createCourseCategoryModel);
    }
}

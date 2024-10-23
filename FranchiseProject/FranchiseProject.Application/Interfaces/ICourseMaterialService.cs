using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ICourseMaterialService
    {
        Task<ApiResponse<bool>> DeleteCourseMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<CourseMaterialViewModel>> GetCourseMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<bool>> UpdateCourseMaterialAsync(Guid materialId, UpdateCourseMaterialModel updateMaterialModel);
        Task<ApiResponse<bool>> CreateCourseMaterialAsync(CreateCourseMaterialModel createMaterialModel);
        Task<ApiResponse<bool>> CreateMaterialArrangeAsync(Guid courseId, List<CreateCourseMaterialArrangeModel> createMaterialArrangeModel);
    }
}

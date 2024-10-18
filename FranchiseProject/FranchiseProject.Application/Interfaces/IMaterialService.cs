using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IMaterialService
    {
        Task<ApiResponse<bool>> DeleteMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<MaterialViewModel>> GetMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<bool>> UpdateMaterialAsync(Guid materialId, UpdateMaterialModel updateMaterialModel);
        Task<ApiResponse<bool>> CreateMaterialAsync(CreateMaterialModel createMaterialModel);
    }
}

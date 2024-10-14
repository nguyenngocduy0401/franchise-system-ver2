using FranchiseProject.Application.Commons;
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
        Task<ApiResponse<List<Material>>> GetAllMaterialAsync();
        Task<ApiResponse<bool>> DeleteMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<Material>> GetMaterialByIdAsync(Guid materialId);
        Task<ApiResponse<bool>> UpdateMaterialAsync(Guid materialId, CreateSlotModel updateSlotModel);
        Task<ApiResponse<bool>> CreateMaterialAsync(CreateSlotModel createSlotModel);
    }
}

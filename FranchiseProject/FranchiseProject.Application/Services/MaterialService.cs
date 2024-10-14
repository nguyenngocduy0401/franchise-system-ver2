using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class MaterialService : IMaterialService
    {
        public Task<ApiResponse<bool>> CreateMaterialAsync(CreateSlotModel createSlotModel)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> DeleteMaterialByIdAsync(Guid materialId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<List<Material>>> GetAllMaterialAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<Material>> GetMaterialByIdAsync(Guid materialId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> UpdateMaterialAsync(Guid materialId, CreateSlotModel updateSlotModel)
        {
            throw new NotImplementedException();
        }
    }
}

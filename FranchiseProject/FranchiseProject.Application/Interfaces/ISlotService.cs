using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ISlotService
    {
        Task<ApiResponse<Pagination<SlotViewModel>>> FilterSlotAsync(FilterSlotModel filterSlotModel);
        Task<ApiResponse<bool>> DeleteSlotByIdAsync(Guid slotId);
        Task<ApiResponse<SlotViewModel>> GetSlotByIdAsync(Guid slotId);
        Task<ApiResponse<bool>> UpdateSlotAsync(Guid slotId, CreateSlotModel updateSlotModel);
        Task<ApiResponse<bool>> CreateSlotAsync(CreateSlotModel createSlotModel);
    }
}

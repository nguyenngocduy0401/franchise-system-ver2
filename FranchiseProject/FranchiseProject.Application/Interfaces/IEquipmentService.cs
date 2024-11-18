using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IEquipmentService
    {
        Task<ApiResponse<bool>> CreateEquipmentAsync(List<EquipmentRequestViewModel> models);

        Task<ApiResponse<byte[]>> ExportEquipmentsToExcelAsync(Guid contractId);

        Task<ApiResponse<bool>> AddEquipmentAfterContractSigningAsync(Guid contractId, List<EquipmentRequestViewModel> equipmentRequests);

        Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentsByAgencyIdAsync(Guid agencyId, int pageIndex, int pageSize);
    }
}

using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IEquipmentService
    {
        Task<ApiResponse<object>> ImportEquipmentsFromExcelAsync(IFormFile file, Guid agencyId);
        Task<ApiResponse<byte[]>> GenerateEquipmentReportAsync(Guid agencyId);
        Task<ApiResponse<bool>> UpdateEquipmentStatusAsync(Guid contractId, List<UpdateEquipmentRangeViewModel> updateModels);
        Task<ApiResponse<Pagination<EquipmentViewModel>>> GetEquipmentByAgencyIdAsync(FilterEquipmentViewModel filter);
    }
}

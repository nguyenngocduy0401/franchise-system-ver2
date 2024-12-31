using FranchiseProject.Application.Commons;
using FranchiseProject.Domain.Enums;
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
        Task<ApiResponse<string>> GenerateEquipmentReportAsync(Guid agencyId);
        Task<ApiResponse<bool>> UpdateEquipmentStatusAsync(Guid equipmentIds, EquipmentStatusEnum equipmentStatus);
        Task<ApiResponse<object>> ImportEquipmentsAfterFranchiseFromExcelAsync(IFormFile file, Guid agencyId);
    }
}

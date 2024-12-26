using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.PackageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IPackageService
    {
        Task<ApiResponse<List<PackageViewModel>>> GetAllStandardPackageByAsync();
        Task<ApiResponse<bool>> CreatePackageAsync(CreatePackageModel createPackageModel);
        Task<ApiResponse<bool>> UpdatePackageAsync(Guid id, UpdatePackageModel updatePackageModel);
        Task<ApiResponse<PackageViewModel>> GetPackageByIdAsync(Guid id);
        Task<ApiResponse<bool>> DeletePackageByIdAsync(Guid id);
        Task<ApiResponse<Pagination<PackageViewModel>>> FilterPackageAsync(FilterPackageModel filterPackageModel);
    }
}

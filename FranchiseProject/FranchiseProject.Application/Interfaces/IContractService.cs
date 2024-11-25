using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IContractService
    {
        Task NotifyCustomersOfExpiringContracts();
        Task<ApiResponse<bool>> CreateContractAsync(CreateContractViewModel create);
        Task<ApiResponse<bool>> UpdateContractAsync(UpdateContractViewModel update, string id);
        Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractViewModelAsync(FilterContractViewModel filter);
        Task<ApiResponse<ContractViewModel>> GetContractByIdAsync(string id);
        Task<ApiResponse<AgencyInfoViewModel>> GetAgencyInfoAsync(Guid agencyId);
    }
}

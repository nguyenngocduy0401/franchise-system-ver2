using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Application.ViewModels.ContractViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IContractService
    {
        Task<ApiResponse<bool>> CreateContractAsync(CreateContractViewModel create);
        Task<ApiResponse<bool>> UpdateContractAsync(UpdateContractViewModel update, string id);
        Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractViewModelAsync(FilterContractViewModel filter);
        Task<ApiResponse<ContractViewModel>> GetContractByIdAsync(string id);

    }
}

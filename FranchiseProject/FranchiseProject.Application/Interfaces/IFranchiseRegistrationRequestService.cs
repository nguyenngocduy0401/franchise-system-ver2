using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IFranchiseRegistrationRequestService
    {
        Task<ApiResponse<bool>> RegisterFranchiseAsync(RegisterFranchiseViewModel regisFranchiseViewModel);
        Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string requestId);
        Task<ApiResponse<Pagination<FranchiseRegistrationRequestsViewModel>>> FilterFranchiseRegistrationRequestAsync(FilterFranchiseRegistrationRequestsViewModel filterModel);
    }
}

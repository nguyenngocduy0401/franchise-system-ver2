using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IConsultationService
    {
<<<<<<< HEAD:FranchiseProject/FranchiseProject.Application/Interfaces/IFranchiseRegistrationRequestService.cs
        Task<ApiResponse<bool>> RegisterFranchiseAsync(RegisterFranchiseViewModel regisFranchiseViewModel);
=======
        Task<ApiResponse<bool>> RegisterConsultationAsync(RegisFranchiseViewModel regisFranchiseViewModel);
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a:FranchiseProject/FranchiseProject.Application/Interfaces/IConsultationService.cs
        Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string requestId);
        Task<ApiResponse<Pagination<FranchiseRegistrationRequestsViewModel>>> FilterConsultationAsync(FilterFranchiseRegistrationRequestsViewModel filterModel);
    }
}

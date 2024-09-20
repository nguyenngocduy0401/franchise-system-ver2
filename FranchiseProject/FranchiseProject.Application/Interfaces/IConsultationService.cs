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
        Task<ApiResponse<bool>> RegisterConsultationAsync(RegisterConsultationViewModel regisFranchiseViewModel);
        Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string requestId);
        Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel);
    }
}

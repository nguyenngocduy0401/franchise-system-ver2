using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IRegisterFormSevice
    {

        Task<ApiResponse<bool>> RegisterConsultationAsync(RegisterConsultationViewModel regisFranchiseViewModel);
        Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string requestId, CustomerStatus customerStatus);
        Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel);
    }
}

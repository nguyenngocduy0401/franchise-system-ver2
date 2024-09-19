using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAgencyService
    {
        Task<ApiResponse<bool>> CreateAgencyAsync(CreateAgencyViewModel create);
        Task<ApiResponse<bool>> UpdateAgencyAsync(CreateAgencyViewModel update,string id);
        Task<ApiResponse<Pagination<AgencyViewModel>>> FilterAgencyAsync(FilterAgencyViewModel filter);
        Task<ApiResponse<AgencyViewModel>> GetAgencyById(string id);
        Task<ApiResponse<bool>> UpdateAgencyStatusAsync(string id, AgencyStatusEnum newStatus);
    }
}

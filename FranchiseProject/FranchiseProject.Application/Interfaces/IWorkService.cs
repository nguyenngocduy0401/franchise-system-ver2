using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IWorkService
    {
        Task<ApiResponse<Pagination<WorkViewModel>>> GetWorksAgencyAsync(FilterWorkByLoginModel filterWorkByLoginModel);
        Task<ApiResponse<bool>> CheckPreWorkAvailable(Work work);
        Task<ApiResponse<bool>> UpdateWorkStatusSubmitByStaffAsync(Guid workId, WorkStatusSubmitEnum workStatusSubmitEnum);
        Task<ApiResponse<bool>> UpdateWorkByStaffAsync(Guid workId, UpdateWorkByStaffModel updateWorkByStaffModel);
        Task<ApiResponse<bool>> CreateWorkAsync(CreateWorkModel createWorkModel);
        Task<ApiResponse<bool>> DeleteWorkByIdAsync(Guid workId);
        Task<ApiResponse<WorkAgencyViewModel>> GetAllWorkByAgencyId(Guid agencyId);
        Task<ApiResponse<bool>> UpdateWorkAsync(Guid workId, UpdateWorkModel updateWorkModel);
        Task<ApiResponse<WorkDetailViewModel>> GetWorkDetailByIdAsync(Guid id);
        Task<ApiResponse<Pagination<WorkViewModel>>> FilterWorksByLogin(FilterWorkByLoginModel filterWorkByLoginModel);
        Task<ApiResponse<bool>> UpdateStatusWorkByIdAsync(Guid workId, WorkStatusEnum status);
        Task<ApiResponse<Pagination<WorkViewModel>>> FilterWorkAsync(FilterWorkModel filterWorkModel);
    }
}

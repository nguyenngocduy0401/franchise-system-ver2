using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IWorkTemplateService
    {
        Task<ApiResponse<WorkTemplateDetailViewModel>> GetWorkTemplateDetailByIdAsync(Guid id);
        Task<ApiResponse<List<WorkTemplateViewModel>>> GetAllWorkTemplateAsync();
        Task<ApiResponse<bool>> UpdateWorkTemplateAsync(Guid workTemplateId, UpdateWorkTemplateModel updateWorkTemplateModel);
        Task<ApiResponse<bool>> CreateWorkTemplateAsync(CreateWorkTemplateModel createWorkTemplateModel);
        Task<ApiResponse<bool>> DeleteWorkTemplateByIdAsync(Guid workTemplateId);
    }
}

using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAssignmentService
    {
        Task<ApiResponse<bool>> CreateAssignmentAsync(CreateAssignmentViewModel assignment);
        Task<ApiResponse<bool>> UpdateAssignmentAsync(CreateAssignmentViewModel update, string assignmentId);
        Task<ApiResponse<AssignmentViewModel>> GetAssignmentByIdAsync(string slotId);
        Task<ApiResponse<bool>> DeleteSlotByIdAsync(string assId);
        Task<ApiResponse<Pagination<AssignmentViewModel>>> GetAssignmentByClassIdAsync(string classId, int pageIndex, int pageSize);
    }
}

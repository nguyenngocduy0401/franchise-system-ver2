using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAssessmentService
    {
        Task<ApiResponse<bool>> DeleteAssessmentByIdAsync(Guid assessmentId);
        Task<ApiResponse<AssessmentViewModel>> GetAssessmentByIdAsync(Guid assessmentId);
        Task<ApiResponse<bool>> UpdateAssessmentAsync(Guid assessmentId, UpdateAssessmentModel updateAssessmentModel);
        Task<ApiResponse<bool>> CreateAssessmentAsync(CreateAssessmentModel createAssessmentModel);
        Task<ApiResponse<bool>> CreateAssessmentArangeAsync(Guid courseId, List<CreateAssessmentArrangeModel> createAssessmentArrangeModel);
    }
}

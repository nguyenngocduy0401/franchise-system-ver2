using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<ApiResponse<bool>> CreateFeedBackAsync(CreateFeedBackViewModel model);
        Task<ApiResponse<FeedBackViewModel>> GetFeedBaByIdAsync(Guid feedbackId);
        Task<ApiResponse<bool>> DeleteFeedBackByIdAsync(Guid id);
        Task<ApiResponse<Pagination<FeedBackViewModel>>> FilterFeedBackAsync(FilterFeedbackViewModel filterModel);
    }
}

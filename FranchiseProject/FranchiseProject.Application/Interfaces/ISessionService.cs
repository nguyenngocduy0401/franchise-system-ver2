using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ISessionService
    {
        Task<ApiResponse<bool>> DeleteSessionByIdAsync(Guid sessionId);
        Task<ApiResponse<SessionViewModel>> GetSessionByIdAsync(Guid sessionId);
        Task<ApiResponse<bool>> UpdateSessionAsync(Guid sessionId, UpdateSessionModel updateSessionModel);
        Task<ApiResponse<bool>> CreateSessionAsync(CreateSessionModel createSessionModel);
        Task<ApiResponse<bool>> CreateSessionArrangeAsync(Guid courseId, List<CreateSessionArrangeModel> createSessionArrangeModel);
    }
}

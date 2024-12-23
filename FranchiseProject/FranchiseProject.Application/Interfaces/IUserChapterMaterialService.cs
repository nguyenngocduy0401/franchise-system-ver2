using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.UserChapterMaterialViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IUserChapterMaterialService
    {
        Task<ApiResponse<UserChapterMaterialModel>> CreateUserChapterMaterialByLoginAsync(CreateUserChapterMaterialModel createUserChapterMaterial);
        Task<ApiResponse<UserChapterMaterialModel>> GetUserChapterMaterialByLoginAsync(Guid userChapterMaterialId);
    }
}

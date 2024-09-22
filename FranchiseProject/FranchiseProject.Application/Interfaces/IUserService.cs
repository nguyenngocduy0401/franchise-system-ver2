using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserViewModel>> GetInfoByLoginAsync();
        Task<ApiResponse<bool>> CreateUserByAdminAsync(CreateUserByAdminModel createUserModel);
        Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync(FilterUserByAdminModel filterUserByAdminModel);
        Task<ApiResponse<bool>> UpdateUserByAdminAsync(string id, UpdateUserByAdminModel updateUserByAdminModel);
        Task<ApiResponse<bool>> DeleteUserByAdminAsync(string id);
        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id);
    }
}

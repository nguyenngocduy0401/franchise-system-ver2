using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.RefreshTokenViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<UserLoginViewModel>> LoginAsync(UserLoginModel userLoginModel);
        Task<ApiResponse<RefreshTokenModel>> RenewTokenAsync(RefreshTokenModel RefreshTokenModel);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<bool>> ResetPasswordAsync(string userName, UserResetPasswordModel userResetPasswordModel);
    }
}

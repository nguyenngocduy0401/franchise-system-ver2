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
    }
}

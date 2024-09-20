using FranchiseProject.Application.ViewModels.RefreshTokenViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserViewModels
{
    public class UserLoginViewModel
    {
        public RefreshTokenModel RefreshTokenModel { get; set; }
        public UserViewModel UserViewModel { get; set; }
    }
}

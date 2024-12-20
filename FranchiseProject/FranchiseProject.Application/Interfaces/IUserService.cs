﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IUserService
    {

        Task<ApiResponse<bool>> UpdateUserByLoginAsync(UpdateUserByLoginModel updateUserByLoginModel);
        Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAgencyManagerAsync(FilterUserByAgencyModel filterUserByAgencyModel);
        Task<ApiResponse<List<CreateUserByAgencyModel>>> CreateListUserByAgencyAsync(IFormFile file);
        Task<ApiResponse<UserViewModel>> GetInfoByLoginAsync();
        Task<ApiResponse<CreateUserByAdminModel>> CreateUserByAdminAsync(CreateUserByAdminModel createUserModel);
        Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync(FilterUserByAdminModel filterUserByAdminModel);
        Task<ApiResponse<bool>> UpdateUserByAdminAsync(string id, UpdateUserByAdminModel updateUserByAdminModel);
        Task<ApiResponse<bool>> BanAndUnbanUserByAdminAsync(string id);
        Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id);
        Task<ApiResponse<bool>> ChangePasswordAsync(UpdatePasswordModel updatePasswordModel);
        Task<ApiResponse<CreateUserViewModel>> CreateUserByAgencyAsync(CreateUserByAgencyModel createUserModel);
        Task<UserLoginModel> GenerateUserCredentials(string fullname);
        Task<ApiResponse<bool>> BanAndUnbanUserByAgencyAsync(string id);
        Task<ApiResponse<bool>> UpdateUserByAgencyAsync(string id, UpdateUserByAgencyModel updateUserByAgencyModel);
    }
}

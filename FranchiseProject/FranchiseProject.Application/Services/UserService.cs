using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdatePasswordModel> _updatePasswordValidator;
        private readonly IValidator<CreateUserByAdminModel> _createUserValidator;
        private readonly IValidator<UpdateUserByAdminModel> _updateUserValidator;

        public UserService(IClaimsService claimsService, IUnitOfWork unitOfWork,
            UserManager<User> userManager, RoleManager<Role> roleManager,
            IMapper mapper, IValidator<UpdatePasswordModel> updatePasswordValidator,
            IValidator<CreateUserByAdminModel> createUserValidator, IValidator<UpdateUserByAdminModel> updateUserValidator)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _updatePasswordValidator = updatePasswordValidator;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
        }
        public async Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync(FilterUserByAdminModel filterUserByAdminModel)
        {
            var response = new ApiResponse<Pagination<UserViewModel>>();
            try
            {
                var filter = (Expression<Func<User, bool>>)(e =>
                    (string.IsNullOrEmpty(filterUserByAdminModel.Search) || e.UserName.Contains(filterUserByAdminModel.Search)
                    || e.Email.Contains(filterUserByAdminModel.Search) || e.PhoneNumber.Contains(filterUserByAdminModel.Search)) &&
                    (!filterUserByAdminModel.AgencyId.HasValue || e.AgencyId == filterUserByAdminModel.AgencyId) &&
                    (!filterUserByAdminModel.ContractId.HasValue || e.ContractId == filterUserByAdminModel.ContractId)
                );
                var rolee = filterUserByAdminModel.Role.ToString();
                var usersPagination = await _unitOfWork.UserRepository.GetFilterAsync(
                    filter: filter,
                    role: filterUserByAdminModel.Role.ToString(),
                    isActive: filterUserByAdminModel.IsActive,
                    pageIndex: filterUserByAdminModel.PageIndex,
                    pageSize: filterUserByAdminModel.PageSize
                );

                if (usersPagination.Items.IsNullOrEmpty())
                {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy người dùng phù hợp!";
                }
                else
                {
                    var userViewModels = _mapper.Map<List<UserViewModel>>(usersPagination.Items);
                    

                    var result = new Pagination<UserViewModel>
                    {
                        PageIndex = usersPagination.PageIndex,
                        PageSize = usersPagination.PageSize,
                        TotalItemsCount = usersPagination.TotalItemsCount,
                        Items = userViewModels
                    };

                    response.Data = result;
                    response.isSuccess = true;
                    response.Message = "Successful!";
                }
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateUserByAdminAsync(CreateUserByAdminModel createUserModel)
        {

            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createUserValidator.ValidateAsync(createUserModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                if (string.IsNullOrEmpty(createUserModel.Role) || !(await _roleManager.RoleExistsAsync(createUserModel.Role))) 
                    throw new Exception("Role does not exist!");

                var user = _mapper.Map<User>(createUserModel);
                var identityResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.ToString());
                await _userManager.AddToRoleAsync(user, createUserModel.Role);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo người dùng thành công";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateUserByAdminAsync(string id, UpdateUserByAdminModel updateUserByAdminModel)
        {

            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateUserValidator.ValidateAsync(updateUserByAdminModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var user =await _userManager.FindByIdAsync(id);
                if (user == null) throw new Exception("Not find user!");
                user.UserName = updateUserByAdminModel.UserName;
                user.PhoneNumber = updateUserByAdminModel.PhoneNumber;
                user.Email = updateUserByAdminModel.Email;
                user.FullName = updateUserByAdminModel.FullName;
                user.DateOfBirth = updateUserByAdminModel.DateOfBirth;
                user.URLImage = updateUserByAdminModel.URLImage;
                user.Gender = updateUserByAdminModel.Gender;
                user.ContractId = string.IsNullOrEmpty(updateUserByAdminModel.ContractId) ? (Guid?)null : Guid.Parse(updateUserByAdminModel.ContractId);
                user.AgencyId = string.IsNullOrEmpty(updateUserByAdminModel.AgencyId) ? (Guid?)null : Guid.Parse(updateUserByAdminModel.AgencyId);
                var isSuccess = await _userManager.UpdateAsync(user);
                if (!isSuccess.Succeeded) throw new Exception("Update fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Cập nhật người dùng thành công";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteUserByAdminAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null) throw new Exception("User not found!");
                if (await _userManager.IsInRoleAsync(user, RolesEnum.Administrator.ToString()))
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không thể khóa tài khoản này!";
                }
                if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow)
                {
                    var lockoutEndDate = DateTimeOffset.MaxValue;
                    await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
                    response.Data = true;
                    response.Message = "Người dùng đã bị cấm.";
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                    response.Data = true;
                    response.Message = "Hủy bỏ lệnh cấm người dùng thành công.";
                }
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<UserViewModel>> GetInfoByLoginAsync()
        {
            var response = new ApiResponse<UserViewModel>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    throw new Exception("User not found!");
                }

                var userViewModel = _mapper.Map<UserViewModel>(user);
                var userRole = await _userManager.GetRolesAsync(user);
                userViewModel.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                response.Data = userViewModel;
                response.isSuccess = true;
                response.Message = "Successful!";
            }
            catch (DbException ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            return response;
        }
        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id)
        {
            var response = new ApiResponse<UserViewModel>();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) throw new Exception("Not found!");
                UserViewModel userViewModel = _mapper.Map<UserViewModel>(user);
                response.Data = userViewModel;
                response.isSuccess = true;
                response.Message = "Successful!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

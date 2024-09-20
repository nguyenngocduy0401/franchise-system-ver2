using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
        private readonly IValidator<CreateUserModel> _createUserValidator;
        private readonly IValidator<UpdateUserModel> _updateUserValidator;

        public UserService(IClaimsService claimsService, IUnitOfWork unitOfWork,
            UserManager<User> userManager, RoleManager<Role> roleManager,
            IMapper mapper, IValidator<UpdatePasswordModel> updatePasswordValidator,
            IValidator<CreateUserModel> createUserValidator, IValidator<UpdateUserModel> updateUserValidator)
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
        /*public async Task<ApiResponse<bool>> CreateUserAsync(CreateUserModel createUserModel)
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
                var attributesToCheck = new List<(string AttributeValue, string AttributeType, string ErrorMessage)>
                {
                    (createUserModel.UserName!, "UserName", "UserName is existed!"),
                    (createUserModel.Email!, "Email", "Email is existed!"),
                    (createUserModel.PhoneNumber!, "PhoneNumber", "PhoneNumber is existed!")
                };
                foreach (var (attributeValue, attributeType, errorMessage) in attributesToCheck)
                {
                    if (await _unitOfWork.UserRepository.CheckUserAttributeExisted(attributeValue, attributeType))
                    {
                        response.Data = false;
                        response.isSuccess = true;
                        response.Message = errorMessage;
                        return response;
                    }
                }
                var user = _mapper.Map<User>(createUserModel);
                user.Wallet = 0;
                var identityResult = await _userManager.CreateAsync(user, user.PasswordHash);
                if (identityResult.Succeeded == false) throw new Exception(identityResult.Errors.ToString());

                if (string.IsNullOrEmpty(createUserModel.Roles))
                {
                    if (!await _roleManager.RoleExistsAsync(RolesEnum.Customer.ToString()))
                    {
                        await _roleManager.CreateAsync(new Role { Name = RolesEnum.Customer.ToString() });
                    }
                    await _userManager.AddToRoleAsync(user, RolesEnum.Customer.ToString());
                }
                else
                {
                    if (!await _roleManager.RoleExistsAsync(createUserModel.Roles))
                    {
                        await _userManager.AddToRoleAsync(user, RolesEnum.Customer.ToString());
                        response.Data = false;
                        response.isSuccess = true;
                        response.Message = "Chọn vai trò không hợp lệ hệ tự động đưa về vai trò khách hàng";
                        return response;
                    }
                    await _userManager.AddToRoleAsync(user, createUserModel.Roles.ToString());
                }

                response.Data = true;
                response.isSuccess = true;
                response.Message = "Register is successful!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }*/
        public async Task<ApiResponse<bool>> DeleteUserAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null) throw new Exception("User not found!");
                if (await _userManager.IsInRoleAsync(user, AppRole.Admin))
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
        /*public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id)
        {
            var response = new ApiResponse<UserViewModel>();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) throw new Exception("Not found!");
                var packageDetail = await _unitOfWork.PackageDetailRepository.GetByUserIdAsync(id);
                UserViewModel userViewModel = _mapper.Map<UserViewModel>(user);
                userViewModel.PackageDetail = _mapper.Map<PackageDetailViewModel>(packageDetail);
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
        }*/
    }
}

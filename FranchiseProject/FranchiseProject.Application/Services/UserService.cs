using AutoMapper;
using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class UserService : IUserService
    {
        #region DI
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ICurrentTime _currentTime;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IValidator<UpdatePasswordModel> _updatePasswordValidator;
        private readonly IValidator<CreateUserByAdminModel> _createUserValidator;
        private readonly IValidator<UpdateUserByAdminModel> _updateUserValidator;
        private readonly IValidator<UpdateUserByAgencyModel> _updateUserByAgencyValidator;
        private readonly IValidator<CreateUserByAgencyModel> _createUserByAgencyModel;
        #endregion
        public UserService(IClaimsService claimsService, IUnitOfWork unitOfWork,
            UserManager<User> userManager, RoleManager<Role> roleManager,
            IMapper mapper, IValidator<UpdatePasswordModel> updatePasswordValidator,
            IValidator<CreateUserByAdminModel> createUserValidator, IValidator<UpdateUserByAdminModel> updateUserValidator,
            IValidator<UpdateUserByAgencyModel> updateUserByAgencyValidator, ICurrentTime currentTime,
            IValidator<CreateUserByAgencyModel> createUserByAgencyModel, IEmailService emailService)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _updatePasswordValidator = updatePasswordValidator;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _updateUserByAgencyValidator = updateUserByAgencyValidator;
            _currentTime = currentTime;
            _createUserByAgencyModel = createUserByAgencyModel;
            _emailService = emailService;
        }
        public async Task<ApiResponse<List<CreateUserByAgencyModel>>> CreateListUserByAgencyAsync(IFormFile file)
        {
            var response = new ApiResponse<List<CreateUserByAgencyModel>>();
            try
            {

                if (file == null || file.Length == 0) throw new Exception("File is empty.");
                var userList = new List<CreateUserByAgencyModel>();

                var data = new List<Dictionary<string, string>>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheets.First();
                        var rows = worksheet.RangeUsed().RowsUsed();

                        var headerRow = rows.First();
                        var headers = headerRow.Cells().Select(c => c.Value.ToString()).ToList();

                        
                        foreach (var row in rows.Skip(1))
                        {
                            var user = new CreateUserByAgencyModel();
                            foreach (var cell in row.Cells())
                            {
                                var columnIndex = cell.Address.ColumnNumber - 1;  

                                switch (columnIndex)
                                {
                                    case 0:  
                                        user.FullName = cell.Value.ToString();
                                        break;
                                    case 1:  
                                        user.Email = cell.Value.ToString();
                                        break;
                                    case 2:  
                                        user.PhoneNumber = cell.Value.ToString();
                                        break;
                                    case 3:
                                        user.Address = cell.Value.ToString();
                                        break;
                                }
                            }
                            userList.Add(user);
                        }
                    }
                }
                response.Data = userList;
                response.isSuccess = true;
                response.Message = "OK";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<CreateUserViewModel>> CreateUserByAgencyAsync(CreateUserByAgencyModel createUserModel)
        {

            var response = new ApiResponse<CreateUserViewModel>();
            try
            {
                ValidationResult validationResult = await _createUserByAgencyModel.ValidateAsync(createUserModel);
                if (!validationResult.IsValid)
                {
                    response.Data = null;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                if (string.IsNullOrEmpty(createUserModel.Role) || !(await _roleManager.RoleExistsAsync(createUserModel.Role)))
                    throw new Exception("Role does not exist!");

                var user = _mapper.Map<User>(createUserModel);

                var userAccount = await GenerateUserCredentials(user.FullName);
                user.UserName = userAccount.UserName;
                user.PasswordHash = userAccount.Password;

                await _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(user, createUserModel.Role);

                var userModel = _mapper.Map<CreateUserViewModel>(user);
                userModel.Password = userAccount.Password;
                var check = await _emailService.SendEmailCreateAccountAsync(userModel);
                if (!check) throw new Exception("Send email fail!");
                response.Data = userModel;
                response.isSuccess = true;
                response.Message = "Tạo người dùng thành công";

            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateUserByAgencyAsync(string id, UpdateUserByAgencyModel updateUserByAgencyModel)
        {

            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateUserByAgencyValidator.ValidateAsync(updateUserByAgencyModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }

                var user = await _userManager.FindByIdAsync(id);
                var userAgencyId = _claimsService.GetCurrentUserId.ToString();
                var userAgency = await _userManager.FindByIdAsync(userAgencyId);
                if (user == null) throw new Exception("User not found!");
                if (userAgency == null) throw new Exception("Login fail!");
                if ((user.AgencyId != userAgency.AgencyId) || user.AgencyId != null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không thể cập nhật tài khoản nằm ngoài phạm vi trung tâm!";
                }

                user = _mapper.Map(updateUserByAgencyModel, user);
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
        public async Task<ApiResponse<bool>> DeleteUserByAgencyAsync(string id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var userAgencyId = _claimsService.GetCurrentUserId.ToString();

                var userAgency = await _userManager.FindByIdAsync(userAgencyId);
                if (user == null) throw new Exception("Not found user");
                if (userAgency == null) throw new Exception("Login fail!");

                if ((user.AgencyId != userAgency.AgencyId) || user.AgencyId != null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không thể khóa tài khoản nằm ngoài phạm vi trung tâm!";
                }
                if (!await _userManager.IsInRoleAsync(user, RolesEnum.Student.ToString()) &&
                    !await _userManager.IsInRoleAsync(user, RolesEnum.Instructor.ToString()))
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
        public async Task<ApiResponse<CreateUserByAdminModel>> CreateUserByAdminAsync(CreateUserByAdminModel createUserModel)
        {

            var response = new ApiResponse<CreateUserByAdminModel>();
            try
            {
                ValidationResult validationResult = await _createUserValidator.ValidateAsync(createUserModel);

                if (!validationResult.IsValid) throw new Exception(string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage)));

                if (string.IsNullOrEmpty(createUserModel.Role) || !(await _roleManager.RoleExistsAsync(createUserModel.Role)))
                    throw new Exception("Role does not exist!");
                var user = _mapper.Map<User>(createUserModel);
                await _unitOfWork.UserRepository.CreateUserAndAssignRoleAsync(user, createUserModel.Role);

                response.Data = createUserModel;
                response.isSuccess = true;
                response.Message = "Tạo người dùng thành công";

            }
            catch (Exception ex)
            {
                response.Data = null;
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
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) throw new Exception("Not found user!");
                user = _mapper.Map(updateUserByAdminModel, user);
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
        public async Task<ApiResponse<bool>> ChangePasswordAsync(UpdatePasswordModel updatePasswordModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updatePasswordValidator.ValidateAsync(updatePasswordModel);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var userId = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new Exception("User does not exist!");
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var checkResetPassword = await _userManager.ResetPasswordAsync(user, resetToken, updatePasswordModel.NewPassword);
                if (!checkResetPassword.Succeeded)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = "Reset password is fail!";
                    return response;
                }
                user.ExpireOTPEmail = null;
                user.OTPEmail = null;
                var checkUpdateUser = await _userManager.UpdateAsync(user);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Đổi mật khẩu thành công!";
                return response;
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        } 
        private async Task<UserLoginModel> GenerateUserCredentials(string fullname)
    {
        var normalizedString = fullname.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        fullname = stringBuilder.ToString();
        var words = fullname.Split(' ');
        string lastName = words.Last();


        var initials = string.Concat(words.Take(words.Length - 1).Select(n => n[0]));
        var year = DateTime.Now.Year.ToString().Substring(2);
        var month = DateTime.Now.Month.ToString();
        var username = lastName + initials + year + month;
        var counter = 1;
        while (await _unitOfWork.UserRepository.CheckUserNameExistAsync(username))
        {
            username = lastName + initials + year + month + counter;
            counter++;
        }
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var password = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return new UserLoginModel { Password = password, UserName = username };
    }
    }
}

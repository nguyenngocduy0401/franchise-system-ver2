using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.RefreshTokenViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;
        public AuthenticationController(IAuthenticationService authenticationService, IEmailService emailService)
        {
            _authenticationService = authenticationService;
            _emailService = emailService;
        }
        
        [SwaggerOperation(Summary = "đăng nhập bằng UserName và Password")]
        [HttpPost("login")]
        public async Task<ApiResponse<UserLoginViewModel>> LoginAsync(UserLoginModel userLoginModel)
        {
            return await _authenticationService.LoginAsync(userLoginModel);
        }
        [HttpPut("new-token")]
        [SwaggerOperation(Summary = "làm mới token")]
        public async Task<ApiResponse<RefreshTokenModel>> RenewTokenAsync(RefreshTokenModel refreshTokenModel)
        {
            return await _authenticationService.RenewTokenAsync(refreshTokenModel);
        }
        [Authorize]
        [SwaggerOperation(Summary = "đăng xuất bằng refreshToken")]
        [HttpDelete("logout")]
        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            return await _authenticationService.LogoutAsync(refreshToken);
        }
        [HttpPost("otp-email")]
        [SwaggerOperation(Summary = "gửi OTP qua email để đổi mật khẩu")]
        public async Task<ApiResponse<bool>> OTPEmailAsync(OTPEmailModel otpEmailModel)
        {
            return await _emailService.SendOTPEmailAsync(otpEmailModel);
        }
        [HttpPut("reset-password/{username}")]
        [SwaggerOperation(Summary = "dùng otp để đổi mật khẩu mới")]
        public async Task<ApiResponse<bool>> ResetPasswordAsync(string username, UserResetPasswordModel userResetPasswordModel)
        {
            return await _authenticationService.ResetPasswordAsync(username, userResetPasswordModel);
        }
    }
}

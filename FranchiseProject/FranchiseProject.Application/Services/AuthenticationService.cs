using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.RefreshTokenViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly SignInManager<User> _signInManager;
        private readonly AppConfiguration _appConfiguration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRedisService _redisService;
        public AuthenticationService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime,
            SignInManager<User> signInManager, AppConfiguration appConfiguration, UserManager<User> userManager,
            RoleManager<Role> roleManager, IRedisService redisService)
        {

            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
            _userManager = userManager;
            _roleManager = roleManager;
            _redisService = redisService;
        }
        public async Task<ApiResponse<RefreshTokenModel>> LoginAsync(UserLoginModel userLoginModel)
        {
            var response = new ApiResponse<RefreshTokenModel>();
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    userLoginModel.UserName, userLoginModel.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _unitOfWork.UserRepository.GetUserByUserNameAndPassword
                        (userLoginModel.UserName, userLoginModel.Password);
                    var userRole = await _userManager.GetRolesAsync(user);

                    var refreshToken = GenerateJsonWebTokenString.GenerateRefreshToken();

                    var token = user.GenerateJsonWebToken(
                        _appConfiguration,
                        _appConfiguration.JwtOptions.Secret,
                        _currentTime.GetCurrentTime(),
                        userRole,
                        refreshToken
                        );
                    var refreshTokenEntity = new RefreshToken
                    {
                        JwtId = token.AccessToken,
                        UserId = user.Id,
                        Token = refreshToken,
                        IsUsed = false,
                        IsRevoked = false,
                        IssuedAt = DateTime.UtcNow,
                        ExpiredAt = DateTime.UtcNow.AddMonths(1)
                    };
                    _unitOfWork.RefreshTokenRepository.UpdateRefreshToken(refreshTokenEntity);
                    var storeRedis = await _redisService.StoreJwtTokenAsync(user.UserName, token.AccessToken);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                    if (isSuccess) {
                        response.Data = token;
                        response.isSuccess = true;
                        response.Message = "Login is successful!";
                    } else throw new Exception("Login is fail!");
                }
                else
                {
                    response.isSuccess = false;
                    response.Message = "Username or password is not correct!";
                }
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

        public async Task<ApiResponse<RefreshTokenModel>> RenewTokenAsync(RefreshTokenModel refreshTokenModel)
        {
            var response = new ApiResponse<RefreshTokenModel>();
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenValidateParam = new TokenValidationParameters
            {

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _appConfiguration.JwtOptions.Issuer,
                ValidAudience = _appConfiguration.JwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.JwtOptions.Secret)),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            try
            {

                var tokenInVerification = jwtTokenHandler.ValidateToken(refreshTokenModel.AccessToken, tokenValidateParam, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        response.isSuccess = false;
                        response.Message = "Refresh token does not exist";
                        return response;
                    }
                }
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = GenerateJsonWebTokenString.ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    response.isSuccess = false;
                    response.Message = "Access token has not yet expired";
                    return response;
                }
                var storedToken = await _unitOfWork.RefreshTokenRepository.GetRefreshTokenByTokenAsync(refreshTokenModel.RefreshToken);
                if (storedToken == null)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token does not exist";
                    return response;
                }
                if (storedToken.IsUsed)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token has been used";
                    return response;
                }
                if (storedToken.IsRevoked)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token has been revoked";
                    return response;
                }

                if (storedToken.JwtId != refreshTokenModel.AccessToken)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token do not match!";
                    return response;
                }

                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _unitOfWork.RefreshTokenRepository.UpdateRefreshToken(storedToken);
                await _unitOfWork.SaveChangeAsync();
                var refreshToken = GenerateJsonWebTokenString.GenerateRefreshToken();
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                var userRole = await _userManager.GetRolesAsync(user);
                var token = user.GenerateJsonWebToken(_appConfiguration,
                        _appConfiguration.JwtOptions.Secret,
                        _currentTime.GetCurrentTime(),
                        userRole,
                        refreshToken
                        );

                var refreshTokenEntity = new RefreshToken
                {
                    JwtId = token.AccessToken,
                    UserId = user.Id,
                    Token = refreshToken,
                    IsUsed = false,
                    IsRevoked = false,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddMonths(1)
                };
                _unitOfWork.RefreshTokenRepository.UpdateRefreshToken(refreshTokenEntity);
                var storeRedis = await _redisService.StoreJwtTokenAsync(user.UserName, token.AccessToken);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                if (!isSuccess)
                {
                    response.isSuccess = false;
                    response.Message = "fail!";
                }
                response.Data = token;
                response.isSuccess = true;
                response.Message = "Refresh Successful!";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;

            }
            return response;
        }
        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var response = new ApiResponse<string>();
            try
            {
                var storedToken = await _unitOfWork.RefreshTokenRepository.GetRefreshTokenByTokenAsync(refreshToken);
                if (storedToken == null)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token does not exist";
                    return response;
                }
                if (storedToken.IsRevoked || storedToken.IsUsed)
                {
                    response.isSuccess = false;
                    response.Message = "Refresh token has been revoked";
                    return response;
                }
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _unitOfWork.RefreshTokenRepository.UpdateRefreshToken(storedToken);
                var user = await _userManager.FindByIdAsync(storedToken.UserId);
                await _redisService.RemoveUserIfExistsAsync(user.UserName);
                await _unitOfWork.SaveChangeAsync();
                response.isSuccess = true;
                response.Message = "Logout Successful!";
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
                return response;

            }
            return response;
        }
    }
}

using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;

namespace FranchiseProject.Application.Services
{
    public class EmailService : IEmailService
    {
        private EmailConfiguration _emailConfiguration;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserResetPasswordModel> _validatorResetPassword;
        public EmailService(EmailConfiguration emailConfiguration, UserManager<User> userManager, IValidator<UserResetPasswordModel> validatorResetPassword)
        {
            _emailConfiguration = emailConfiguration;
            _userManager = userManager;
            _validatorResetPassword = validatorResetPassword;
        }

        public async Task<ApiResponse<bool>> SendOTPEmailAsync(OTPEmailModel otpEmailModel)
        {
            var response = new ApiResponse<bool>();
            if (string.IsNullOrEmpty(otpEmailModel.Username))
            {
                response.Data = false;
                response.isSuccess = true;
                response.Message = "Không được bỏ trống Username!";
                return response;
            }
            var user = await _userManager.FindByNameAsync(otpEmailModel.Username);
            if (user == null)
            {
                response.Data = false;
                response.isSuccess = true;
                response.Message = "Username không tồn tại!";
                return response;
            }

            string otp = new Random().Next(0, 1000000).ToString("D6");
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress
                ("FutureTech", _emailConfiguration.From));
            emailMessage.To.Add(new MailboxAddress
                (user.Email, user.Email));
            emailMessage.Subject = "Your OTP Code";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = $"Your OTP code is: {otp}"
            };

            using var client = new SmtpClient();
            try
            {
                user.OTPEmail = otp;
                user.ExpireOTPEmail = DateTime.Now.AddMinutes(5);
                var checkUpdate = await _userManager.UpdateAsync(user);
                if (!checkUpdate.Succeeded)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = "Cannot update OTP email!";
                    return response;
                }
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                // Loại bỏ cơ chế xác thực XOAUTH2  MailKit.Security.SecureSocketOptions.StartTls
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
                await client.SendAsync(emailMessage);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "OTP sent successfully.";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
            return response;
        }

    }
}

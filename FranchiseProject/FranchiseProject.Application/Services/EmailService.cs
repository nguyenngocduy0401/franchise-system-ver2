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
        private AppConfiguration _appConfiguration;
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserResetPasswordModel> _validatorResetPassword;
        public EmailService(AppConfiguration appConfiguration, UserManager<User> userManager, IValidator<UserResetPasswordModel> validatorResetPassword)
        {
            _appConfiguration = appConfiguration;
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
            if (user.Email == null)
            {
                response.Data = false;
                response.isSuccess = true;
                response.Message = "Người dùng không có emai!";
                return response;
            }
            string otp = new Random().Next(0, 1000000).ToString("D6");
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress
                ("futuretech-noreply", _appConfiguration.EmailConfiguration.From));
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
                await client.ConnectAsync(_appConfiguration.EmailConfiguration.SmtpServer, _appConfiguration.EmailConfiguration.Port, true);
                // Loại bỏ cơ chế xác thực XOAUTH2  MailKit.Security.SecureSocketOptions.StartTls
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_appConfiguration.EmailConfiguration.Username, _appConfiguration.EmailConfiguration.Password);
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
        public async Task<ApiResponse<bool>> SendRegistrationSuccessEmailAsync(string email)
        {
            var response = new ApiResponse<bool>();
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("futuretech-noreply", _appConfiguration.EmailConfiguration.From));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = "No-reply: Registration Successful";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<p>Congratulations! You have successfully registered for consultation.</p><p>Thank you for choosing us!</p>"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_appConfiguration.EmailConfiguration.SmtpServer, _appConfiguration.EmailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_appConfiguration.EmailConfiguration.Username, _appConfiguration.EmailConfiguration.Password);
                await client.SendAsync(emailMessage);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Registration success email sent.";
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
            }
            return response;
        }
        public async Task<ApiResponse<bool>> SendContractEmailAsync(string agencyEmail, string contractUrl)
        {
            var response = new ApiResponse<bool>();
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("No-reply:FutureTech", _appConfiguration.EmailConfiguration.From));
            emailMessage.To.Add(new MailboxAddress(agencyEmail, agencyEmail));
            emailMessage.Subject = ": Your Contract Document";

            // Create email body with a link to the contract document
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"<p>Your contract has been created successfully. You can download it <a href='{contractUrl}'>here</a>.</p>"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_appConfiguration.EmailConfiguration.SmtpServer, _appConfiguration.EmailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_appConfiguration.EmailConfiguration.Username, _appConfiguration.EmailConfiguration.Password);
                await client.SendAsync(emailMessage);
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Contract email sent successfully.";
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
            }
            return response;
        }
    }
}
    


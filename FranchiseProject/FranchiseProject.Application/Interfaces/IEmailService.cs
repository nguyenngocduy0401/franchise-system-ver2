using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.EmailViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IEmailService
    {
        Task<ApiResponse<bool>> SendOTPEmailAsync(OTPEmailModel otpEmailModel);
        Task<ApiResponse<bool>> SendRegistrationSuccessEmailAsync(string email);
        Task<ApiResponse<bool>> SendContractEmailAsync(string agencyEmail, string contractUrl);
    }
}

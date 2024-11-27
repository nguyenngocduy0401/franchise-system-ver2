using FluentValidation;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.AutheticationValidator
{
    public class UserResetPasswordValidator : AbstractValidator<UserResetPasswordModel>
    {
        public UserResetPasswordValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Tên người dùng không được để trống.");

            RuleFor(x => x.OTP)
                .NotEmpty()
                .WithMessage("Mã OTP không được để trống.")
                .Length(6)
                .WithMessage("Mã OTP phải có 6 ký tự.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Mật khẩu mới không được để trống.")
                .MinimumLength(6)
                .WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
                .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).+$")
                .WithMessage("Mật khẩu của bạn phải chứa ít nhất một chữ cái và một số.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Mật khẩu xác nhận không đúng.");
        }
    }
}

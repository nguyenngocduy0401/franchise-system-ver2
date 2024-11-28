using FluentValidation;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.UserValidator
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordModel>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Mật khẩu cũ không được để trống.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự.")
                .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).+$")
                .WithMessage("Mật khẩu mới của bạn phải chứa ít nhất một chữ cái và một số.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("Mật khẩu xác nhận không đúng!");
        }
    }
}

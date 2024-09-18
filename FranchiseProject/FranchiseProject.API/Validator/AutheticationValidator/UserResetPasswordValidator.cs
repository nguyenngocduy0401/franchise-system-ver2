using FluentValidation;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.AutheticationValidator
{
    public class UserResetPasswordValidator : AbstractValidator<UserResetPasswordModel>
    {
        public UserResetPasswordValidator()
        {
            RuleFor(x => x.OTP).NotEmpty().Length(6);
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6)
               .WithMessage("Password must be at least 6 characters long!")
           .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).+$")
               .WithMessage("Your password must contain at least one number!");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.ConfirmPassword)
                .WithMessage("Your password confirmed is wrong!");
        }
    }
}

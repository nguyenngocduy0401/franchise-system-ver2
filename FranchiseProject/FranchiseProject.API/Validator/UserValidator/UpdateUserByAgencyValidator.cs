using FluentValidation;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.UserValidator
{
    public class UpdateUserByAgencyValidator : AbstractValidator<UpdateUserByAgencyModel>
    {
        public UpdateUserByAgencyValidator()
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .WithMessage("Email is invalid format!");
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^0[0-9]{9}$")
                .WithMessage("The phone number must have 10 digits and start with 0!");
        }
    }
}
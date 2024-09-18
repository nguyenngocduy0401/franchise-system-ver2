using FluentValidation;
using FranchiseProject.Application.ViewModels.AgencyViewModel;

namespace FranchiseProject.API.Validator.AgencyValidation
{
    public class CreateAgencyValidator : AbstractValidator<CreateAgencyViewModel>

    {
        public CreateAgencyValidator() {

            RuleFor(x => x.Name)
                .MaximumLength(50)
                .WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(50)
                .WithMessage("Address cannot exceed 50 characters.");
            RuleFor(x => x.City)
               .MaximumLength(50)
               .WithMessage("City cannot exceed 50 characters.");
            RuleFor(x => x.District)
               .MaximumLength(50)
               .WithMessage("District cannot exceed 50 characters.");
            RuleFor(x => x.Ward)
              .MaximumLength(50)
              .WithMessage("Ward cannot exceed 50 characters.");
            RuleFor(x => x.PhoneNumber).
                  NotEmpty().Matches(@"^0[0-9]{9}$")
                  .WithMessage("The phone number must have 10 digits and start with 0!");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");
        }
    }
}

using FluentValidation;
using FranchiseProject.Application.ViewModels.AgencyViewModel;

namespace FranchiseProject.API.Validator.AgencyValidation
{
    public class RegisFranchiseViewModelValidator : AbstractValidator<RegisterConsultationViewModel>
    {
        public RegisFranchiseViewModelValidator()
        {
          
         

            RuleFor(x => x.CustomerName)
                .MaximumLength(50)
                .WithMessage("CustomerName cannot exceed 100 characters.");

            RuleFor(x => x.PhoneNumber).
                  NotEmpty().Matches(@"^0[0-9]{9}$")
                  .WithMessage("The phone number must have 10 digits and start with 0!");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is not valid.");
        }
    }
}

using FluentValidation;
using FranchiseProject.Application.ViewModels.AgencyViewModel;

namespace FranchiseProject.API.Validator.AgencyValidation
{
<<<<<<< HEAD:FranchiseProject/FranchiseProject.API/Validator/AgencyValidator/RegisFranchiseViewModelValidator.cs
    public class RegisFranchiseViewModelValidator : AbstractValidator<RegisterFranchiseViewModel>
=======
    public class RegisFranchiseViewModelValidator : AbstractValidator<RegisterConsultation>
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a:FranchiseProject/FranchiseProject.API/Validator/AgencyValidation/RegisFranchiseViewModelValidator.cs
    {
        public RegisFranchiseViewModelValidator()
        {
          
            RuleFor(x => x.Address)
                .MaximumLength(100)
                .WithMessage("Address cannot exceed 100 characters.");

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

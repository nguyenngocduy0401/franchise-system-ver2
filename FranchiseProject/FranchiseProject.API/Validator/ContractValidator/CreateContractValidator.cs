using FluentValidation;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using System.Diagnostics.Contracts;

namespace FranchiseProject.API.Validator.ContractValidator
{
    public class CreateContractValidator :AbstractValidator<CreateContractViewModel>
    {
        public CreateContractValidator()
        {

          

            RuleFor(x => x.Duration)
                .Must(d => new[] { 1, 2, 3, 5, 10 }.Contains(d)) // Year options
                .WithMessage("Duration must be one of the following: 1, 2, 3, 5, or 10 years.");

            RuleFor(x => x.Description)
                .NotNull()
                .WithMessage("Description cannot be null.");

         
            RuleFor(x => x.AgencyId)
                .NotNull()
                .WithMessage("Agency ID cannot be null.");
        }

    }
}

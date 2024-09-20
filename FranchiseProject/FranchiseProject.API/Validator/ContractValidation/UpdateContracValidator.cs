using FluentValidation;
using FranchiseProject.Application.ViewModels.ContractViewModel;

namespace FranchiseProject.API.Validator.ContractValidation
{
    public class UpdateContracValidator: AbstractValidator<UpdateContractViewModel>
    {
        public UpdateContracValidator()
        {

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount must be greater than or equal to 0.");

            RuleFor(x => x.Duration)
                .Must(d => new[] { 1, 2, 3, 5, 10 }.Contains(d)) // Year options
                .WithMessage("Duration must be one of the following: 1, 2, 3, 5, or 10 years.");

            RuleFor(x => x.Description)
                .NotNull()
                .WithMessage("Description cannot be null.");

            RuleFor(x => x.TermsAndCondition)
                .NotNull()
                .WithMessage("Terms and Conditions cannot be null.");
                }
    }
}

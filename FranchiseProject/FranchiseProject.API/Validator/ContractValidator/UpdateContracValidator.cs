using FluentValidation;
using FranchiseProject.Application.ViewModels.ContractViewModels;

namespace FranchiseProject.API.Validator.ContractValidator
{
    public class UpdateContracValidator : AbstractValidator<UpdateContractViewModel>
    {
        public UpdateContracValidator()
        {


            RuleFor(x => x.Title)
                .MaximumLength(50)
                .WithMessage("Title must be less than 100.")
                .NotNull()
                .WithMessage("Title cannot be null.");
            RuleFor(x => x.StartTime)
                    .NotEmpty().WithMessage("Start Time must not be empty");
            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End Time must not be empty");

            RuleFor(x => x.RevenueSharePercentage)
                .NotEmpty().WithMessage("Revenue Share Percentage must not be empty")
                .GreaterThan(0).WithMessage("Revenue Share Percentage must be greater than 0.")
                .LessThan(100).WithMessage("Revenue Share Percentage must be less than 0");
        }
    }
}

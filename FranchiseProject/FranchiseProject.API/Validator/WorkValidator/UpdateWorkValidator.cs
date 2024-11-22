using FluentValidation;
using FranchiseProject.Application.ViewModels.WorkViewModels;

namespace FranchiseProject.API.Validator.WorkValidator
{
    public class UpdateWorkValidator : AbstractValidator<UpdateWorkModel>
    {
        public UpdateWorkValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .MaximumLength(300);
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .LessThanOrEqualTo(x => x.EndDate)
                .GreaterThanOrEqualTo(new DateTime(2020, 1, 1))
                .WithMessage("Start date must be in the future.");
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.Now.AddYears(1))
                .WithMessage("Start date must be within one year from today.");
        }
    }
}

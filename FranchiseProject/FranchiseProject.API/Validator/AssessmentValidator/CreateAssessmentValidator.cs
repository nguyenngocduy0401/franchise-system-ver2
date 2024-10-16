using FluentValidation;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;

namespace FranchiseProject.API.Validator.AssessmentValidator
{
    public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentModel>
    {
        public CreateAssessmentValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Type must be not empty and less than 100 characters");
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Content must be not empty and less than 100 characters");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Weight must be greater than 0");
            RuleFor(x => x.CompletionCriteria)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CompletionCriteria must be greater than 0");
        }
    }
}

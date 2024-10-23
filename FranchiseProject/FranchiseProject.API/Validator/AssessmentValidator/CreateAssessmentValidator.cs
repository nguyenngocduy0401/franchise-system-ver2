using FluentValidation;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;

namespace FranchiseProject.API.Validator.AssessmentValidator
{
    public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentModel>
    {
        public CreateAssessmentValidator()
        {   
            RuleFor(x => x.Number)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(10);
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.CompletionCriteria)
                .GreaterThanOrEqualTo(0);
        }
    }
}

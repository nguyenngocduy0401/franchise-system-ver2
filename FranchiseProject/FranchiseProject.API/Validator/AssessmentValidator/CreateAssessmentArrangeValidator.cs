using FluentValidation;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;

namespace FranchiseProject.API.Validator.AssessmentValidator
{
    public class CreateAssessmentArrangeValidator : AbstractValidator<List<CreateAssessmentArrangeModel>>
    {
        public CreateAssessmentArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleAssessmentArrangeValidator());
        }
    }
}

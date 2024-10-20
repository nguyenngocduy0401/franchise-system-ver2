using FluentValidation;
using FranchiseProject.API.Validator.AssessmentValidator;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class CreateChapterArrangeValidator : AbstractValidator<List<CreateChapterArrangeModel>>
    {
        public CreateChapterArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleChapterArrangeValidator());
        }
    }
}

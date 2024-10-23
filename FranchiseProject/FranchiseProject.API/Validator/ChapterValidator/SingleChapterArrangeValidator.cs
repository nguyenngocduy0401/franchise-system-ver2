using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class SingleChapterArrangeValidator : AbstractValidator<CreateChapterArrangeModel>
    {
        public SingleChapterArrangeValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(x => x.Topic)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}

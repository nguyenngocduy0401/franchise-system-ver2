using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class CreateChapterValidator : AbstractValidator<CreateChapterModel>
    {
        public CreateChapterValidator()
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

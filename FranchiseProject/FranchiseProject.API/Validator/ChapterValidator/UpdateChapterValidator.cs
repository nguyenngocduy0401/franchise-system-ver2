using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class UpdateChapterValidator : AbstractValidator<UpdateChapterModel>
    {
        public UpdateChapterValidator()
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


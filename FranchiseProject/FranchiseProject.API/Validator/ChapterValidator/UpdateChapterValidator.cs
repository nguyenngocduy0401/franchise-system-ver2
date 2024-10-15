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
                .GreaterThan(0)
                .WithMessage("Number must be not empty and greater than 0");
            RuleFor(x => x.Topic)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Topic must be not empty and less than 150 characters");
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Description must be not empty and less than 500 characters");
        }
    }
}


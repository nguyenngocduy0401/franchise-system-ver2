using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;

namespace FranchiseProject.API.Validator.ChapterMaterialValidator
{
    public class UpdateChapterMaterialValidator : AbstractValidator<UpdateChapterMaterialModel>
    {
        public UpdateChapterMaterialValidator()
        {
            RuleFor(e => e.Number)
                .GreaterThanOrEqualTo(0);
            RuleFor(e => e.URL)
                .NotEmpty()
                .MaximumLength(250);
            RuleFor(e => e.Description)
                .MaximumLength(500);
        }
    }
}

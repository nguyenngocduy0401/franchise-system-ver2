using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterMaterialValidator
{
    public class SingleChapterMaterialArrangeValidator : AbstractValidator<CreateChapterMaterialArrangeModel>
    {
        public SingleChapterMaterialArrangeValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(x => x.URL)
                    .NotEmpty()
                    .MaximumLength(200);
            RuleFor(x => x.Description)
                    .NotEmpty()
                    .MaximumLength(500);
        }
    }
}

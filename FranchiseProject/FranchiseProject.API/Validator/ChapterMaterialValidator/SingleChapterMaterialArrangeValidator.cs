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
                .WithMessage("Số không được để trống.")
                .GreaterThan(0)
                .WithMessage("Số phải lớn hơn 0.");

            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL không được để trống.")
                .MaximumLength(200)
                .WithMessage("URL không được dài quá 200 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống.")
                .MaximumLength(500)
                .WithMessage("Mô tả không được dài quá 500 ký tự.");
        }
    }
}

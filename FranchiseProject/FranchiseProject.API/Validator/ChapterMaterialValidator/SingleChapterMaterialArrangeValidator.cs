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
            RuleFor(e => e.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(250)
                .WithMessage("Tiêu đề không được dài quá 250 ký tự.");
            /*RuleFor(x => x.URLFile)
                .NotEmpty()
                .WithMessage("URL không được để trống.")
                .MaximumLength(200)
                .WithMessage("URL không được dài quá 200 ký tự.");*/

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống.")
                .MaximumLength(5000)
                .WithMessage("Mô tả không được dài quá 5000 ký tự.");
        }
    }
}

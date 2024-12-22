using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;

namespace FranchiseProject.API.Validator.ChapterMaterialValidator
{
    public class UpdateChapterMaterialValidator : AbstractValidator<UpdateChapterMaterialModel>
    {
        public UpdateChapterMaterialValidator()
        {
            RuleFor(e => e.Number)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Số không được nhỏ hơn 0.");
            RuleFor(e => e.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(250)
                .WithMessage("Tiêu đề không được dài quá 250 ký tự.");
            RuleFor(e => e.URL)
                .NotEmpty()
                .WithMessage("URL không được để trống.")
                .MaximumLength(250)
                .WithMessage("URL không được dài quá 250 ký tự.");

            RuleFor(e => e.Description)
                .MaximumLength(5000)
                .WithMessage("Mô tả không được dài quá 5000 ký tự.");
        }
    }
}

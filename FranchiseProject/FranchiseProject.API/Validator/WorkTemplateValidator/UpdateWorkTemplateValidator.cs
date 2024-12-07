using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;

namespace FranchiseProject.API.Validator.WorkTemplateValidator
{
    public class UpdateWorkTemplateValidator : AbstractValidator<UpdateWorkTemplateModel>
    {
        public UpdateWorkTemplateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(150)
                .WithMessage("Tiêu đề không được dài hơn 150 ký tự.");

            RuleFor(x => x.Description.GetTextWithoutHtml())
                .MaximumLength(2000)
                .WithMessage("Mô tả không được dài hơn 2000 ký tự.");

            RuleFor(x => x.StartDaysOffset)
                .NotEmpty()
                .WithMessage("Khoảng cách ngày bắt đầu không được để trống.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Khoảng cách ngày bắt đầu phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.DurationDays)
                .NotEmpty()
                .WithMessage("Thời gian thực hiện không được để trống.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Thời gian thực hiện phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Loại không được để trống.");
        }
    }
}

using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.WorkViewModels;

namespace FranchiseProject.API.Validator.WorkValidator
{
    public class UpdateWorkValidator : AbstractValidator<UpdateWorkModel>
    {
        public UpdateWorkValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(150)
                .WithMessage("Tiêu đề không được dài hơn 150 ký tự.");

            RuleFor(x => x.Description.GetTextWithoutHtml())
                .MaximumLength(2000)
                .WithMessage("Mô tả không được dài hơn 2000 ký tự.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Ngày bắt đầu không được để trống.")
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.")
                .GreaterThanOrEqualTo(new DateTime(2020, 1, 1))
                .WithMessage("Ngày bắt đầu phải từ ngày 01/01/2020 trở đi.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("Ngày kết thúc không được để trống.")
                .LessThanOrEqualTo(DateTime.Now.AddYears(1))
                .WithMessage("Ngày kết thúc phải trong vòng một năm kể từ hôm nay.");
        }
    }
}

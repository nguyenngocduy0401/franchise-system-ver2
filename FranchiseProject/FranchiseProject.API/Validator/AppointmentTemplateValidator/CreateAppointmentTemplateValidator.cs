using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels;

namespace FranchiseProject.API.Validator.AppointmentTemplateValidator
{
    public class CreateAppointmentTemplateValidator : AbstractValidator<CreateAppointmentTemplateModel>
    {
        public CreateAppointmentTemplateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(150)
                .WithMessage("Tiêu đề không được dài quá 150 ký tự.");

            RuleFor(x => x.Description.GetTextWithoutHtml())
                .MaximumLength(1000)
                .WithMessage("Mô tả không được dài quá 1000 ký tự.");
            RuleFor(x => x.StartDaysOffset)
                .NotEmpty()
                .WithMessage("Khoảng cách ngày bắt đầu không được để trống.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Khoảng cách ngày bắt đầu phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.DurationHours)
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

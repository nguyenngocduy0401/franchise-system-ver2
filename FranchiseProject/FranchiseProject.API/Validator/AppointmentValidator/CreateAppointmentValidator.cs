using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;

namespace FranchiseProject.API.Validator.AppointmentValidator
{
    public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentModel>
    {
        public CreateAppointmentValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Tiêu đề không được để trống.")
                .MaximumLength(150)
                .WithMessage("Tiêu đề không được dài quá 150 ký tự.");

            RuleFor(x => x.Description.GetTextWithoutHtml())
                .MaximumLength(1000)
                .WithMessage("Mô tả không được dài quá 1000 ký tự.");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Thời gian bắt đầu không được để trống.")
                .LessThanOrEqualTo(x => x.EndTime)
                .WithMessage("Thời gian bắt đầu phải trước hoặc bằng thời gian kết thúc.");
        }
    }
}

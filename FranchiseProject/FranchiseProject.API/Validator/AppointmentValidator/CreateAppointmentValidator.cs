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
                .MaximumLength(150);
            RuleFor(x => x.Description.GetTextWithoutHtml())
                .MaximumLength(1000);
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .LessThanOrEqualTo(x => x.EndTime);
        }
    }
}

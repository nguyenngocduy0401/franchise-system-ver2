using FluentValidation;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;

namespace FranchiseProject.API.Validator.AppointmentValidator
{
    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentModel>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .MaximumLength(1000);
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .LessThanOrEqualTo(x => x.EndTime);
        }
    }
}

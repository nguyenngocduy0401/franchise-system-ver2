using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleDateRangeValidator:AbstractValidator<CreateClassScheduleDateRangeViewModel>
    {
        public CreateClassScheduleDateRangeValidator()
        {
            RuleFor(x => x.Room).NotEmpty().WithMessage("Room is not empty!");

            RuleFor(x => x.startDate)
           .Must(BeAValidStartDate)
           .WithMessage("\r\nThe start date cannot be a date in the past.");
        }
        private bool BeAValidStartDate(DateOnly date)
        {
          
            return date >= DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

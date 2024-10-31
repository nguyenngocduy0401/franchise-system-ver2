using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleValidor : AbstractValidator<CreateClassScheduleViewModel>
    {
        public CreateClassScheduleValidor() 
        {
            RuleFor(x => x.Room).NotEmpty().WithMessage("Room is not empty!");
            RuleFor(x => x.Date)
               .Must(BeAValidStartDate)
               .WithMessage("\r\nThe start date cannot be a date in the past.");
        }
        private bool BeAValidStartDate(string date)
        {
            DateOnly? date1 = DateOnly.Parse(date);
            return date1 >= DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

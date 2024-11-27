using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleValidor : AbstractValidator<CreateClassScheduleViewModel>
    {
        public CreateClassScheduleValidor() 
        {
            RuleFor(x => x.Room)
                .NotEmpty()
                .WithMessage("Phòng không được để trống!");

            RuleFor(x => x.Date)
                .Must(BeAValidStartDate)
                .WithMessage("Ngày bắt đầu không được là ngày trong quá khứ.");
        }
        private bool BeAValidStartDate(string date)
        {
            DateOnly? date1 = DateOnly.Parse(date);
            return date1 >= DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

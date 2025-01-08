using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleDateRangeValidator:AbstractValidator<CreateClassScheduleDateRangeViewModel>
    {
        public CreateClassScheduleDateRangeValidator()
        {
     

            RuleFor(x => x.startDate)
                .Must(BeAValidStartDate)
                .WithMessage("Ngày bắt đầu không được là ngày trong quá khứ.");
        }
        private bool BeAValidStartDate(DateOnly date)
        {
          
            return date >= DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

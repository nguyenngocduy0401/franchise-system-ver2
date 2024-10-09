using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleDateRangeValidator:AbstractValidator<CreateClassScheduleDateRangeViewModel>
    {
        public CreateClassScheduleDateRangeValidator()
        {
            RuleFor(x => x.Room).NotEmpty().WithMessage("Room is not empty!");
        }
    }
}

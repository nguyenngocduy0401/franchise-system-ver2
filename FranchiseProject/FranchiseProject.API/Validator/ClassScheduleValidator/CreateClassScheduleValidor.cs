using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.ClassScheduleValidator
{
    public class CreateClassScheduleValidor : AbstractValidator<CreateClassScheduleViewModel>
    {
        public CreateClassScheduleValidor() 
        {
            RuleFor(x => x.Room).NotEmpty().WithMessage("Room is not empty!");
                
        }
    }
}

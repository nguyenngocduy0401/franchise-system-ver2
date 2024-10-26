using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassViewModel;

namespace FranchiseProject.API.Validator.ClassValidator
{
    public class CreateClassValidator : AbstractValidator<CreateClassViewModel>
    {
        public CreateClassValidator() 
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name  is required.")
            .Length(2, 100).WithMessage("Name  must be between 2 and 100 characters.");
            RuleFor(x => x.Capacity)
                .NotEmpty().WithMessage("Capacity is required.")
                .GreaterThan(0).WithMessage("Capacity must be geater than 0");
           
        
        }
    }
}

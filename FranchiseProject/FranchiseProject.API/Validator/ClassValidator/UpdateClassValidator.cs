using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;

namespace FranchiseProject.API.Validator.ClassValidator
{
    public class UpdateClassValidator : AbstractValidator<UpdateClassViewModel>
    {
        public UpdateClassValidator()
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
using FluentValidation;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.TermViewModel;

namespace FranchiseProject.API.Validator.TermValidator
{
    public class CreateTermValidator: AbstractValidator<CreateTermViewModel>
    {
        public CreateTermValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start Date is required.");
            RuleFor(x => x.EndDate)
               .NotEmpty().WithMessage("End Date is required.");
        }
    }
}

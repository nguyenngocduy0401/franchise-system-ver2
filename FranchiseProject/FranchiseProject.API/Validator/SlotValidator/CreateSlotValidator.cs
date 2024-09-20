using FluentValidation;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.SlotValidator
{
    public class CreateSlotValidator : AbstractValidator<CreateSlotModel>
    {
        public CreateSlotValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("End time must be greater than start time.");
        }
    }
}


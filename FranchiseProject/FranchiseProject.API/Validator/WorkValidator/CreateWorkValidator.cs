using FluentValidation;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.WorkViewModels;

namespace FranchiseProject.API.Validator.WorkValidator
{
    public class CreateWorkValidator : AbstractValidator<CreateWorkModel>
    {
        private readonly ICurrentTime _currentTime;
        public CreateWorkValidator(ICurrentTime currentTime)
        {
            _currentTime = currentTime;
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .MaximumLength(1000);
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(_currentTime.GetCurrentTime())
                .WithMessage("Start date must be in the future.");
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .LessThanOrEqualTo(_currentTime.GetCurrentTime().AddYears(1))
                .WithMessage("Start date must be within one year from today.");
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .LessThanOrEqualTo(_currentTime.GetCurrentTime().AddYears(1))
                .WithMessage("Start date must be within one year from today.");
            RuleFor(x => x.Type)
                .NotEmpty();
        }
    }
}

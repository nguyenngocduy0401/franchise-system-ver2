using FluentValidation;
using FranchiseProject.Application.ViewModels.SessionViewModels;

namespace FranchiseProject.API.Validator.SessionValidator
{
    public class SingleSessionArrangeValidator : AbstractValidator<CreateSessionArrangeModel>
    {
        public SingleSessionArrangeValidator()
        {
                RuleFor(x => x.Number)
                    .GreaterThan(0);
                RuleFor(x => x.Topic)
                    .NotEmpty()
                    .MaximumLength(150);
                RuleFor(x => x.Description)
                    .MaximumLength(500);
        }
    }
}

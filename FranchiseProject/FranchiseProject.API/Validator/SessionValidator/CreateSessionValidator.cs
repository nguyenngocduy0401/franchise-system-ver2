using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;

namespace FranchiseProject.API.Validator.SessionValidator
{
    public class CreateSessionValidator : AbstractValidator<CreateSessionModel>
    {
        public CreateSessionValidator()
        {
            RuleFor(x => x.Number)
                .GreaterThan(0)
                .WithMessage("Number must be greater than 0");
            RuleFor(x => x.Topic)
                .NotEmpty()
                .MaximumLength(150)
                .WithMessage("Topic must be not empty and less than 150 characters");
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must be less than 500 characters");
        }
    }
}
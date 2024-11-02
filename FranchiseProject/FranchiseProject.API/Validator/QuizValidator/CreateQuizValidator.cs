using FluentValidation;
using FranchiseProject.Application.ViewModels.QuizViewModels;

namespace FranchiseProject.API.Validator.QuizValidator
{
    public class CreateQuizValidator : AbstractValidator<CreateQuizModel>
    {
        public CreateQuizValidator()
        {
            RuleFor(x => x.Title)
               .MaximumLength(100);
            RuleFor(x => x.Description)
               .MaximumLength(500);
            RuleFor(x => x.Quantity)
               .GreaterThanOrEqualTo(1)
               .LessThan(100);
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(1)
                .LessThan(360);
            RuleFor(x => x.ChapterId)
                .NotEmpty();
        }
    }
}

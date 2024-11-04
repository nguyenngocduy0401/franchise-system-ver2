using FluentValidation;
using FranchiseProject.Application.ViewModels.QuizViewModels;

namespace FranchiseProject.API.Validator.QuizValidator
{
    public class UpdateQuizValidator : AbstractValidator<UpdateQuizModel>
    {
        public UpdateQuizValidator()
        {
            RuleFor(x => x.Title)
               .MaximumLength(100);
            RuleFor(x => x.Description)
               .MaximumLength(500);
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(1)
                .LessThan(360);
        }
    }
}

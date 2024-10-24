using FluentValidation;
using FranchiseProject.Application.ViewModels.QuestionViewModels;

namespace FranchiseProject.API.Validator.QuestionValidator
{
    public class CreateQuestionArrangeValidator : AbstractValidator<List<CreateQuestionArrangeModel>>
    {
        public CreateQuestionArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleQuestionArrangeValidator());
        }
    }
}

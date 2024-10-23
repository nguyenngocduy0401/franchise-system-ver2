using FluentValidation;
using FranchiseProject.API.Validator.ChapterValidator;
using FranchiseProject.Application.ViewModels.SessionViewModels;

namespace FranchiseProject.API.Validator.SessionValidator
{
    public class CreateSessionArrangeValidator : AbstractValidator<List<CreateSessionArrangeModel>>
    {
        public CreateSessionArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleSessionArrangeValidator());
        }
    }
}

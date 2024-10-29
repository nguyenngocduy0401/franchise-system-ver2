using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class CreateListChapterValidator : AbstractValidator<List<CreateChapterModel>>
    {
        public CreateListChapterValidator()
        {
            RuleForEach(x => x).SetValidator(new CreateChapterValidator());
        }
    }
}

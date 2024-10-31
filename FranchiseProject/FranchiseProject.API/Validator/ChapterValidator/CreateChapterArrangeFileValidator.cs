using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class CreateChapterArrangeFileValidator : AbstractValidator<List<CreateChapterFileModel>>
    {
        public CreateChapterArrangeFileValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleChapterArrangeFileValidator());
        }
    }
}
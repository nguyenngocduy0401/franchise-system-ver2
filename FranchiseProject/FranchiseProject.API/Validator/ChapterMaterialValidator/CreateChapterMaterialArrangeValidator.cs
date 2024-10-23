using FluentValidation;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;

namespace FranchiseProject.API.Validator.ChapterMaterialValidator
{
    public class CreateChapterMaterialArrangeValidator : AbstractValidator<List<CreateChapterMaterialArrangeModel>>
    {
        public CreateChapterMaterialArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleChapterMaterialArrangeValidator());
        }
    }
}

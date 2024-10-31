using FluentValidation;
using FranchiseProject.API.Validator.ChapterMaterialValidator;
using FranchiseProject.API.Validator.QuestionValidator;
using FranchiseProject.Application.ViewModels.ChapterViewModels;

namespace FranchiseProject.API.Validator.ChapterValidator
{
    public class SingleChapterArrangeFileValidator : AbstractValidator<CreateChapterFileModel>
    {
        public SingleChapterArrangeFileValidator()
        {
            RuleFor(x => x.Number)
               .NotEmpty()
               .GreaterThan(0);
            RuleFor(x => x.Topic)
                .NotEmpty()
                .MaximumLength(150);
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);
            RuleFor(x => x.ChapterMaterials)
            .ForEach(material => {
                material.SetValidator(new SingleChapterMaterialArrangeValidator());
            });
            RuleFor(x => x.Questions)
            .ForEach(q => {
                q.SetValidator(new SingleQuestionArrangeValidator());
            });
        }
    }
}

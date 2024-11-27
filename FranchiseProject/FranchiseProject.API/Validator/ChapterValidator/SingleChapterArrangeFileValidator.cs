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
                .WithMessage("Số không được để trống.")
                .GreaterThan(0)
                .WithMessage("Số phải lớn hơn 0.");

            RuleFor(x => x.Topic)
                .NotEmpty()
                .WithMessage("Chủ đề không được để trống.")
                .MaximumLength(150)
                .WithMessage("Chủ đề không được dài quá 150 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống.")
                .MaximumLength(500)
                .WithMessage("Mô tả không được dài quá 500 ký tự.");

            RuleFor(x => x.ChapterMaterials)
                .ForEach(material =>
                {
                    material.SetValidator(new SingleChapterMaterialArrangeValidator());
                });

            RuleFor(x => x.Questions)
                .ForEach(q =>
                {
                    q.SetValidator(new SingleQuestionArrangeValidator());
                });
        }
    }
}

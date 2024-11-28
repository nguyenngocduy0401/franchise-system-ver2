using FluentValidation;
using FranchiseProject.API.Validator.ChapterMaterialValidator;
using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;

namespace FranchiseProject.API.Validator.QuestionOptionValidator
{
    public class SingleQuestionOptionArrangeValidator : AbstractValidator<CreateQuestionOptionArrangeModel>
    {
        public SingleQuestionOptionArrangeValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống.");
        }
    }
}

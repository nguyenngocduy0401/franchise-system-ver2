using FluentValidation;
using FranchiseProject.API.Validator.ChapterMaterialValidator;
using FranchiseProject.API.Validator.QuestionOptionValidator;
using FranchiseProject.Application.ViewModels.QuestionViewModels;

namespace FranchiseProject.API.Validator.QuestionValidator
{
    public class SingleQuestionArrangeValidator : AbstractValidator<CreateQuestionArrangeModel>
    {
        public SingleQuestionArrangeValidator()
        {
            RuleFor(x => x.Description)
               .NotEmpty();
            RuleFor(x => x.QuestionOptions)
            .ForEach(material =>
            {
                material.SetValidator(new SingleQuestionOptionArrangeValidator());
            });
        }
    }
}

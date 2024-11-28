using FluentValidation;
using FranchiseProject.API.Validator.QuestionOptionValidator;
using FranchiseProject.Application.ViewModels.QuestionViewModels;

namespace FranchiseProject.API.Validator.QuestionValidator
{
    public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionModel>
    {
        public UpdateQuestionValidator() 
        {
            RuleFor(x => x.Description)
               .NotEmpty()
               .WithMessage("Mô tả không được để trống.");;
            RuleFor(x => x.QuestionOptions)
            .ForEach(material =>
            {
                material.SetValidator(new SingleQuestionOptionArrangeValidator());
            });
        }
    }
}

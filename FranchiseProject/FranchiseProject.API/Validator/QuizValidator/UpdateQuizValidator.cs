using FluentValidation;
using FranchiseProject.Application.ViewModels.QuizViewModels;

namespace FranchiseProject.API.Validator.QuizValidator
{
    public class UpdateQuizValidator : AbstractValidator<UpdateQuizModel>
    {
        public UpdateQuizValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(1).WithMessage("Thời gian phải lớn hơn hoặc bằng 1.")
                .LessThan(360).WithMessage("Thời gian phải nhỏ hơn 360.");
        }
    }
}

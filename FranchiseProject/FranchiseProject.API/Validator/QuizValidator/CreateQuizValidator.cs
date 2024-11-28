using FluentValidation;
using FranchiseProject.Application.ViewModels.QuizViewModels;

namespace FranchiseProject.API.Validator.QuizValidator
{
    public class CreateQuizValidator : AbstractValidator<CreateQuizModel>
    {
        public CreateQuizValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Số lượng phải lớn hơn hoặc bằng 1.")
                .LessThan(100).WithMessage("Số lượng phải nhỏ hơn 100.");
            RuleFor(x => x.Duration)
                .GreaterThanOrEqualTo(1).WithMessage("Thời gian phải lớn hơn hoặc bằng 1.")
                .LessThan(360).WithMessage("Thời gian phải nhỏ hơn 360.");
            RuleFor(x => x.ChapterId)
                .NotEmpty().WithMessage("ID chương không được để trống.");
        }
    }
}

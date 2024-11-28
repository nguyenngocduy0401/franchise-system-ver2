using FluentValidation;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;

namespace FranchiseProject.API.Validator.AssessmentValidator
{
    public class UpdateAssessmentValidator : AbstractValidator<UpdateAssessmentModel>
    {
        public UpdateAssessmentValidator()
        {
            RuleFor(x => x.Number)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Số không được nhỏ hơn 0.");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Loại không được để trống.")
                .MaximumLength(100)
                .WithMessage("Loại không được dài quá 100 ký tự.");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Nội dung không được để trống.")
                .MaximumLength(100)
                .WithMessage("Nội dung không được dài quá 100 ký tự.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Số lượng không được nhỏ hơn 0.");

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Cân nặng không được nhỏ hơn 0.");

            RuleFor(x => x.CompletionCriteria)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Tiêu chí hoàn thành không được nhỏ hơn 0.");
        }
    }
}
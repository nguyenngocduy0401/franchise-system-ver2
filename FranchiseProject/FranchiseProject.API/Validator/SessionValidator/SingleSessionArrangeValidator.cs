using FluentValidation;
using FranchiseProject.Application.ViewModels.SessionViewModels;

namespace FranchiseProject.API.Validator.SessionValidator
{
    public class SingleSessionArrangeValidator : AbstractValidator<CreateSessionArrangeModel>
    {
        public SingleSessionArrangeValidator()
        {
            RuleFor(x => x.Number)
                .GreaterThan(0).WithMessage("Số phải lớn hơn 0.");
            RuleFor(x => x.Topic)
                .NotEmpty().WithMessage("Chủ đề không được để trống.")
                .MaximumLength(150).WithMessage("Chủ đề không được vượt quá 150 ký tự.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
        }
    }
}

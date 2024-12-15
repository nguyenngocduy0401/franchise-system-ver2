using FluentValidation;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;

namespace FranchiseProject.API.Validator.SyllabusValidator
{
    public class UpdateSyllabusValidator : AbstractValidator<UpdateSyllabusModel>
    {
        public UpdateSyllabusValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Mô tả không được để trống.")
                .MaximumLength(500)
                .WithMessage("Mô tả phải ít hơn 500 ký tự.");

            RuleFor(x => x.TimeAllocation)
                .NotEmpty()
                .WithMessage("Phân bổ thời gian không được để trống.")
                .MaximumLength(500)
                .WithMessage("Phân bổ thời gian phải ít hơn 500 ký tự.");

            RuleFor(x => x.ToolsRequire)
                .NotEmpty()
                .WithMessage("Công cụ yêu cầu không được để trống")
                .MaximumLength(500)
                .WithMessage("Công cụ yêu cầu phải ít hơn 500 ký tự.");

            RuleFor(x => x.MinAvgMarkToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Điểm trung bình tối thiểu để vượt qua phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Scale)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Thang điểm phải lớn hơn hoặc bằng 0.");
        }
    }
}

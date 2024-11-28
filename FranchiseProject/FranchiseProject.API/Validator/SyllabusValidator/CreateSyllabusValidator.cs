using FluentValidation;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;

namespace FranchiseProject.API.Validator.SyllabusValidator
{
    public class CreateSyllabusValidator : AbstractValidator<CreateSyllabusModel>
    {
        public CreateSyllabusValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Mô tả không được để trống và phải ít hơn 500 ký tự.");

            RuleFor(x => x.TimeAllocation)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Phân bổ thời gian không được để trống và phải ít hơn 500 ký tự.");

            RuleFor(x => x.ToolsRequire)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("Công cụ yêu cầu không được để trống và phải ít hơn 500 ký tự.");

            RuleFor(x => x.MinAvgMarkToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Điểm trung bình tối thiểu để vượt qua phải lớn hơn hoặc bằng 0.");

            RuleFor(x => x.Scale)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Thang điểm phải lớn hơn hoặc bằng 0.");

        }
    }
}

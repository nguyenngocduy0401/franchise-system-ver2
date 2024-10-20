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
                .WithMessage("Description must be not empty and less than 150 characters");
            RuleFor(x => x.TimeAllocation)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("TimeAllocation must be  not empty and less than 500 characters");
            RuleFor(x => x.ToolsRequire)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("ToolsRequire must be  not empty and less than 500 characters");
            RuleFor(x => x.MinAvgMarkToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MinAvgMarkToPass must be greater than or equal to 0 points");
            RuleFor(x => x.Scale)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Scale must be greater than or equal to 0 points");

        }
    }
}

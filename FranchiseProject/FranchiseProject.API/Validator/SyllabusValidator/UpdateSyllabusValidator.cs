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
                .MaximumLength(150)
                .WithMessage("Name must be not empty and less than 150 characters");
            RuleFor(x => x.MinAvgMarkToPass)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MinAvgMarkToPass must be greater than or equal to 0 points");
            RuleFor(x => x.TimeAllocation)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("TimeAllocation must be  not empty and less than 500 characters");
            RuleFor(x => x.ToolsRequire)
                .NotEmpty()
                .MaximumLength(500)
                .WithMessage("ToolsRequire must be  not empty and less than 500 characters");
        }
    }
}

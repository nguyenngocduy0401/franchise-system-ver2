using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;

namespace FranchiseProject.API.Validator.MaterialValidator
{
    public class SingleMaterialArrangeValidator : AbstractValidator<CreateMaterialArrangeModel>
    {
        public SingleMaterialArrangeValidator()
        {
            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required.");
        }
    }
}

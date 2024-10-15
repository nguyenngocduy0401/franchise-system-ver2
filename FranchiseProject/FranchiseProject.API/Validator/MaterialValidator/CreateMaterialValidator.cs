using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;

namespace FranchiseProject.API.Validator.MaterialValidator
{
    public class CreateMaterialValidator : AbstractValidator<CreateMaterialModel>
    {
        public CreateMaterialValidator()
        {
            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required.");
        }
    }
}

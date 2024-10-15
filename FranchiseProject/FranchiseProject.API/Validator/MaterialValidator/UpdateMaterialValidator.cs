using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;

namespace FranchiseProject.API.Validator.MaterialValidator
{
    public class UpdateMaterialValidator : AbstractValidator<UpdateMaterialModel>
    {
        public UpdateMaterialValidator()
        {
            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required.");
        }
    }
}
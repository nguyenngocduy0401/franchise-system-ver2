using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;

namespace FranchiseProject.API.Validator.MaterialValidator
{
    public class CreateMaterialArrangeModelValidator : AbstractValidator<List<CreateMaterialArrangeModel>>
    {
        public CreateMaterialArrangeModelValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleMaterialArrangeValidator());
        }
    }
}
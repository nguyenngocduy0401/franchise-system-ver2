using FluentValidation;
using FranchiseProject.Application.ViewModels.MaterialViewModels;

namespace FranchiseProject.API.Validator.MaterialValidator
{
    public class CreateMaterialArrangeValidator : AbstractValidator<List<CreateMaterialArrangeModel>>
    {
        public CreateMaterialArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleMaterialArrangeValidator());
        }
    }
}
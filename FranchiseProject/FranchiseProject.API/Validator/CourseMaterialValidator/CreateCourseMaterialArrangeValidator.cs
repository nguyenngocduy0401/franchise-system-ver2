using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;

namespace FranchiseProject.API.Validator.CourseMaterialValidator
{
    public class CreateCourseMaterialArrangeValidator : AbstractValidator<List<CreateCourseMaterialArrangeModel>>
    {
        public CreateCourseMaterialArrangeValidator()
        {
            RuleForEach(x => x).SetValidator(new SingleCourseMaterialArrangeValidator());
        }
    }
}
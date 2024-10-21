using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;

namespace FranchiseProject.API.Validator.CourseMaterialValidator
{
    public class SingleCourseMaterialArrangeValidator : AbstractValidator<CreateCourseMaterialArrangeModel>
    {
        public SingleCourseMaterialArrangeValidator()
        {
            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required.");
        }
    }
}

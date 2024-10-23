using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;

namespace FranchiseProject.API.Validator.CourseMaterialValidator
{
    public class CreateCourseMaterialValidator : AbstractValidator<CreateCourseMaterialModel>
    {
        public CreateCourseMaterialValidator()
        {
            RuleFor(x => x.URL)
                .NotEmpty()
                .WithMessage("URL is required.");
        }
    }
}

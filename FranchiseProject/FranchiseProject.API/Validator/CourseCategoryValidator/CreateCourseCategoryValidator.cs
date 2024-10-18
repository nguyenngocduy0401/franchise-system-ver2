using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;

namespace FranchiseProject.API.Validator.CourseCategoryValidator
{
    public class CreateCourseCategoryValidator : AbstractValidator<CreateCourseCategoryModel>
    {
        public CreateCourseCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

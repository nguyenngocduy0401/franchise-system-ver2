using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;

namespace FranchiseProject.API.Validator.CourseCategoryValidator
{
    public class UpdateCourseCategoryValidator : AbstractValidator<UpdateCourseCategoryModel>
    {
        public UpdateCourseCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

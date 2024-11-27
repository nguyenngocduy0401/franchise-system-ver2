using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;

namespace FranchiseProject.API.Validator.CourseCategoryValidator
{
    public class UpdateCourseCategoryValidator : AbstractValidator<UpdateCourseCategoryModel>
    {
        public UpdateCourseCategoryValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(150)
            .WithMessage("Tên danh mục không được dài quá 150 ký tự.");
        }
    }
}

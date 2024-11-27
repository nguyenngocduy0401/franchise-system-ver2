using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;

namespace FranchiseProject.API.Validator.CourseCategoryValidator
{
    public class CreateCourseCategoryValidator : AbstractValidator<CreateCourseCategoryModel>
    {
        public CreateCourseCategoryValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(150)
            .WithMessage("Tên danh mục không được dài quá 150 ký tự.");
        }
    }
}

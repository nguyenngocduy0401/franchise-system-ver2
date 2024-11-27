using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;

namespace FranchiseProject.API.Validator.CourseValidator
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseModel>
    {
        public CreateCourseValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty()
                .WithMessage("Tên khóa học không được để trống.")
                .MaximumLength(150)
                .WithMessage("Tên khóa học không được dài quá 150 ký tự.");

            RuleFor(e => e.Description)
                .NotEmpty()
                .WithMessage("Mô tả khóa học không được để trống.")
                .MaximumLength(500)
                .WithMessage("Mô tả khóa học không được dài quá 500 ký tự.");

            RuleFor(e => e.NumberOfLession)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Số bài học phải lớn hơn hoặc bằng 0.");

            RuleFor(e => e.Price)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Giá phải lớn hơn hoặc bằng 0.")
                .LessThan(100000000)
                .WithMessage("Giá không được lớn hơn 100 triệu.");
        }
    }
}

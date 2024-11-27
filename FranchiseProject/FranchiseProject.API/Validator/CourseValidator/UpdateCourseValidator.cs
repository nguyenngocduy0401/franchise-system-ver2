using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseViewModels;

namespace FranchiseProject.API.Validator.CourseValidator
{
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseModel>
    {
        public UpdateCourseValidator()
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

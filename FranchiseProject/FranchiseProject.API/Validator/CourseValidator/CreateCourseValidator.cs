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
                .MaximumLength(150);
            RuleFor(e => e.Description)
                .NotEmpty()
                .MaximumLength(500);
            RuleFor(e => e.NumberOfLession)
                .GreaterThanOrEqualTo(0);
            RuleFor(e => e.Price)
                .GreaterThanOrEqualTo(0)
                .LessThan(100000000);
        }
    }
}

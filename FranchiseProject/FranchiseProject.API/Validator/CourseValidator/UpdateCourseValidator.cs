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

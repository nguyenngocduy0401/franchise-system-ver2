using FluentValidation;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModels;

namespace FranchiseProject.API.Validator.StudentValidator
{
    public class RegisterCourseValidator : AbstractValidator<RegisterCourseViewModel>
    {
        public RegisterCourseValidator()
        {
            RuleFor(x => x.StudentName)
            .NotEmpty().WithMessage("Student name is required.")
            .Length(2, 100).WithMessage("Student name must be between 2 and 100 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d+$").WithMessage("Phone number must be numeric.")
                .Length(10, 15).WithMessage("Phone number must be between 10 and 15 digits.");
        }
    }
}

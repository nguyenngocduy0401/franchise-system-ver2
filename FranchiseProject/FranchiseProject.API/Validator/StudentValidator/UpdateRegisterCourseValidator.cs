using FluentValidation;
using FranchiseProject.Application.ViewModels.StudentViewModels;

namespace FranchiseProject.API.Validator.StudentValidator
{
    public class UpdateRegisterCourseValidator : AbstractValidator<UpdateRegisterCourseViewModel>
    {
        public UpdateRegisterCourseValidator() {
            RuleFor(x => x.StudentName)
                 .NotEmpty().WithMessage("Student name is required.")
                 .Length(2, 100).WithMessage("Student name must be between 2 and 100 characters.");
            RuleFor(x => x.DateTime)
                .NotEmpty().WithMessage("DateTime is required.");
               
           
        }  
    }
}

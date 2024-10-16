using FluentValidation;
using FranchiseProject.Application.ViewModels.StudentViewModel;

namespace FranchiseProject.API.Validator.StudentValidator
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentViewModel>
    {
        public CreateStudentValidator()
    {
        RuleFor(x => x.StudentName).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address time is required.");
        RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Address time is required.");
            RuleFor(x => x.DateOfBirth)
              .NotEmpty().WithMessage("DateOfBirth time is required.");
         RuleFor(x=>x.PhoneNumber).NotEmpty().Matches(@"^0[0-9]{9}$")
                .WithMessage("The phone number must have 10 digits and start with 0!");
        }
}
}

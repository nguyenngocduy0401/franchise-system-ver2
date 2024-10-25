using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;

namespace FranchiseProject.API.Validator.PaymentValidator
{
    public class PaymentStudentValidator : AbstractValidator<CreateStudentPaymentViewModel>
    {
        public PaymentStudentValidator() {
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(1, 100).WithMessage("Title must be between 1 and 100 characters.");

        /*    RuleFor(x => x.StudentName)
                .NotEmpty().WithMessage("Student Name is required.")
                .Length(1, 50).WithMessage("Student Name must be between 1 and 50 characters.");*/

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Amount)
                .NotNull().WithMessage("Amount is required.")
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

           
        }
    }
}

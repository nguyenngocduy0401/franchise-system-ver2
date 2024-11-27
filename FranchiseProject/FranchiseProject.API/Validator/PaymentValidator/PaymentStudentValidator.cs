using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;

namespace FranchiseProject.API.Validator.PaymentValidator
{
    public class PaymentStudentValidator : AbstractValidator<CreateStudentPaymentViewModel>
    {
        public PaymentStudentValidator() {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề là bắt buộc.")
                .Length(1, 100).WithMessage("Tiêu đề phải có độ dài từ 1 đến 100 ký tự.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

            RuleFor(x => x.Amount)
                .NotNull().WithMessage("Số tiền là bắt buộc.")
                .GreaterThan(0).WithMessage("Số tiền phải lớn hơn 0.");


        }
    }
}

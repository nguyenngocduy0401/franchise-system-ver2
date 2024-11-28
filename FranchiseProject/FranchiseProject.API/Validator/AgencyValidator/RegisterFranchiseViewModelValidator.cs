using FluentValidation;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;

namespace FranchiseProject.API.Validator.AgencyValidation
{
    public class RegisterFranchiseViewModelValidator : AbstractValidator<RegisterConsultationViewModel>
    {
        public RegisterFranchiseViewModelValidator()
        {
            RuleFor(x => x.CustomerName)
              .MaximumLength(50)
              .WithMessage("Tên khách hàng không được vượt quá 50 ký tự.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^0[0-9]{9}$")
                .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email là bắt buộc.")
                .EmailAddress()
                .WithMessage("Email không hợp lệ.");
        }
    }
}

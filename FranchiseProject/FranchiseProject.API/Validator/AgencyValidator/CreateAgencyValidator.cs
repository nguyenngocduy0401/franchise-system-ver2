using FluentValidation;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;

namespace FranchiseProject.API.Validator.AgencyValidation
{
    public class CreateAgencyValidator : AbstractValidator<CreateAgencyViewModel>

    {
        public CreateAgencyValidator() {

            RuleFor(x => x.Name)
                .MaximumLength(50)
                .WithMessage("Tên không được vượt quá 50 ký tự.");
            RuleFor(x => x.Address)
                .MaximumLength(50)
                .WithMessage("Địa chỉ không được vượt quá 50 ký tự.");
            RuleFor(x => x.City)
               .MaximumLength(50)
               .WithMessage("Thành phố không được vượt quá 50 ký tự.");
            RuleFor(x => x.District)
               .MaximumLength(50)
               .WithMessage("Quận không được vượt quá 50 ký tự.");
            RuleFor(x => x.Ward)
              .MaximumLength(50)
              .WithMessage("Phường không được vượt quá 50 ký tự.");
            RuleFor(x => x.PhoneNumber).
                  NotEmpty().Matches(@"^0[0-9]{9}$")
                  .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0.");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email là bắt buộc.")
                .EmailAddress()
                .WithMessage("Email không hợp lệ.");
        }
    }
}

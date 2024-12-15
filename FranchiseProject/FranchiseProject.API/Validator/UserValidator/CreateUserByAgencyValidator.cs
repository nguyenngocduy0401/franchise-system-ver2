using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Enums;

namespace FranchiseProject.API.Validator.UserValidator
{
    public class CreateUserByAgencyValidator : AbstractValidator<CreateUserByAgencyModel>
    {
        public CreateUserByAgencyValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Tên đầy đủ là bắt buộc.");

            RuleFor(x => x.Role)
               .NotEmpty()
               .WithMessage("Vai trò không được để trống.")
               .Must(role => role == RolesEnum.Student.ToString() || role == RolesEnum.Instructor.ToString() || role == RolesEnum.AgencyStaff.ToString())
               .WithMessage("Vai trò phải là học sinh, giáo viên hướng dẫn, hoặc nhân viên trung tâm.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email không được để trống!")
                .EmailAddress()
                .WithMessage("Email không hợp lệ.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^0[0-9]{9}$")
                .WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0.");
        }
    }
}

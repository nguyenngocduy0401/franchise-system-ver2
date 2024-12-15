using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FranchiseProject.API.Validator.UserValidator
{
    public class CreateUserValidator : AbstractValidator<CreateUserByAdminModel>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Tên người dùng không được để trống.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Họ và tên không được để trống.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Vai trò không được để trống.")
                .NotEqual(RolesEnum.Administrator.ToString())
                .WithMessage("Vai trò không được là quản trị viên.");

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

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6)
                .WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
                .Matches(@"^(?=.*[a-zA-Z])(?=.*\d).+$")
                .WithMessage("Mật khẩu của bạn phải chứa ít nhất một chữ cái và một số.");

            RuleFor(x => x.PasswordConfirm)
                .NotEmpty()
                .WithMessage("Mật khẩu xác nhận không được để trống.")
                .Equal(x => x.Password)
                .WithMessage("Mật khẩu xác nhận không đúng!");
        }
    }
}

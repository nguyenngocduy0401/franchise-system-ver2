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
                 .NotEmpty().WithMessage("Tên học sinh là bắt buộc.")
                 .Length(2, 100).WithMessage("Tên học sinh phải có độ dài từ 2 đến 100 ký tự.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email là bắt buộc.")
                .EmailAddress().WithMessage("Email phải hợp lệ.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại là bắt buộc.")
                .Matches(@"^\d+$").WithMessage("Số điện thoại phải là chữ số.")
                .Length(10, 15).WithMessage("Số điện thoại phải có độ dài từ 10 đến 15 chữ số.");
        }
    }
}

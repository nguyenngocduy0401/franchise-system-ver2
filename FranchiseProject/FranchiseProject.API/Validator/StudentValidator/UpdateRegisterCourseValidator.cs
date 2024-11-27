using FluentValidation;
using FranchiseProject.Application.ViewModels.StudentViewModels;

namespace FranchiseProject.API.Validator.StudentValidator
{
    public class UpdateRegisterCourseValidator : AbstractValidator<UpdateRegisterCourseViewModel>
    {
        public UpdateRegisterCourseValidator() {
            RuleFor(x => x.StudentName)
                .NotEmpty().WithMessage("Tên học sinh là bắt buộc.")
                .Length(2, 100).WithMessage("Tên học sinh phải có độ dài từ 2 đến 100 ký tự.");

            RuleFor(x => x.DateTime)
                .NotEmpty().WithMessage("Ngày giờ là bắt buộc.");


        }  
    }
}

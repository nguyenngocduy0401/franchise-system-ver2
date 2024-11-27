using FluentValidation;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;

namespace FranchiseProject.API.Validator.ClassValidator
{
    public class UpdateClassValidator : AbstractValidator<UpdateClassViewModel>
    {
        public UpdateClassValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên là bắt buộc.")
                .Length(2, 100).WithMessage("Tên phải có độ dài từ 2 đến 100 ký tự.");

            RuleFor(x => x.Capacity)
                .NotEmpty().WithMessage("Sức chứa là bắt buộc.")
                .GreaterThan(0).WithMessage("Sức chứa phải lớn hơn 0.");


        }
    }
}
using FluentValidation;
using FranchiseProject.Application.ViewModels.PackageViewModels;

namespace FranchiseProject.API.Validator.PackageValidator
{
    public class UpdatePackageValidator : AbstractValidator<UpdatePackageModel>
    {
        public UpdatePackageValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên gói là bắt buộc.")
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá tiền phải lớn hơn hoặc bằng 0");
            RuleFor(x => x.NumberOfUsers)
               .GreaterThanOrEqualTo(0).WithMessage("Số lượng người dùngs phải lớn hơn hoặc bằng 0");
        }
    }
}

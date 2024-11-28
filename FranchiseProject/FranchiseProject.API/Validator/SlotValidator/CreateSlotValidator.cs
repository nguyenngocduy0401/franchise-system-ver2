using FluentValidation;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;

namespace FranchiseProject.API.Validator.SlotValidator
{
    public class CreateSlotValidator : AbstractValidator<CreateSlotModel>
    {
        public CreateSlotValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên là bắt buộc.");
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Thời gian bắt đầu là bắt buộc.");
            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("Thời gian kết thúc là bắt buộc.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
        }
    }
}


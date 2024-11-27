using FluentValidation;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;

namespace FranchiseProject.API.Validator.FeedBackValidator
{
    public class CreateFeedBackValidator :AbstractValidator<CreateFeedBackViewModel>
    {
        public CreateFeedBackValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề là bắt buộc.")
                .Length(1, 50).WithMessage("Tiêu đề phải có độ dài từ 1 đến 50 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả là bắt buộc.")
                .Length(1, 250).WithMessage("Mô tả phải có độ dài từ 1 đến 250 ký tự.");
        }

    }
}

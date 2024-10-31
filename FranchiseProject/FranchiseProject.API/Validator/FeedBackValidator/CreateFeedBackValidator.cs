using FluentValidation;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;

namespace FranchiseProject.API.Validator.FeedBackValidator
{
    public class CreateFeedBackValidator :AbstractValidator<CreateFeedBackViewModel>
    {public CreateFeedBackValidator()
        {
            RuleFor(x => x.Title)
           .NotEmpty().WithMessage("Title is required.")
           .Length(1, 50).WithMessage("Title must be between 1 and 50 characters.");
            RuleFor(x => x.Description)
           .NotEmpty().WithMessage("Title is required.")
           .Length(1, 250).WithMessage("Description must be between 1 and 250 characters.");

        }

    }
}

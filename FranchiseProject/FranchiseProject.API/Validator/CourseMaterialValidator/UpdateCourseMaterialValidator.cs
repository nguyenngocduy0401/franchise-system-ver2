using FluentValidation;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;

namespace FranchiseProject.API.Validator.CourseMaterialValidator
{
    public class UpdateCourseMaterialValidator : AbstractValidator<UpdateCourseMaterialModel>
    {
        public UpdateCourseMaterialValidator()
        {
            RuleFor(x => x.URL)
            .NotEmpty()
            .WithMessage("URL không được để trống.");
        }
    }
}
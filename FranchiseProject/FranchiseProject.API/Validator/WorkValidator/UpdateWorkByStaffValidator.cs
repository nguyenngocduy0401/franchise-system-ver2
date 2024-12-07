using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.WorkViewModels;

namespace FranchiseProject.API.Validator.WorkValidator
{
    public class UpdateWorkByStaffValidator : AbstractValidator<UpdateWorkByStaffModel>
    {
        public UpdateWorkByStaffValidator()
        {
            RuleFor(x => x.Report.GetTextWithoutHtml())
                 .MaximumLength(10000)
                 .WithMessage("Báo cáo không được dài hơn 10000 ký tự.");

            RuleFor(x => x.ReportImageURL)
                .MaximumLength(500)
                .WithMessage("URL báo cáo không được dài hơn 500 ký tự.");
        }
    }
}

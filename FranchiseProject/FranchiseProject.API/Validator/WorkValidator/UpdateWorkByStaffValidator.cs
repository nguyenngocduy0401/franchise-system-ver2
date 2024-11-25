using FluentValidation;
using FranchiseProject.Application.ViewModels.WorkViewModels;

namespace FranchiseProject.API.Validator.WorkValidator
{
    public class UpdateWorkByStaffValidator : AbstractValidator<UpdateWorkByStaffModel>
    {
        public UpdateWorkByStaffValidator()
        {
            RuleFor(x => x.Report)
                .MaximumLength(3000);
            RuleFor(x => x.ReportImageURL)
                .MaximumLength(500);
        }
    }
}

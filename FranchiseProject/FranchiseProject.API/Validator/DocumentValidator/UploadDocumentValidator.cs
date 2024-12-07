using FluentValidation;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FranchiseProject.API.Validator.DocumentValidator
{
    public class UploadDocumentValidator: AbstractValidator<UploadDocumentViewModel>
    {
        public UploadDocumentValidator() {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề không được để trống!")
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");

            RuleFor(x => x.URLFile)
                .NotEmpty().WithMessage("URLFile không được để trống.");

            RuleFor(x => x.AgencyId)
                .NotEmpty().WithMessage("AgencyId không được để trống.");

         
        }
        private bool BeAFutureDate(DateOnly? date)
        {
            return date.HasValue && date > DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

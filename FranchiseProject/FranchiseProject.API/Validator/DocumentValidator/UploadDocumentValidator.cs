using FluentValidation;
using FranchiseProject.Application.ViewModels.DocumentViewModels;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FranchiseProject.API.Validator.DocumentValidator
{
    public class UploadDocumentValidator: AbstractValidator<UploadDocumentViewModel>
    {
        public UploadDocumentValidator() {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title cannot be null!").MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            RuleFor(X => X.URLFile).NotEmpty().WithMessage("URLFile cannot be null");
            
            RuleFor(x=> x.AgencyId).NotEmpty().WithMessage("AgencyId cannot be null");
            RuleFor(x=>x.ExpirationDate)
                    .Must(BeAFutureDate).WithMessage("Date must be in the future");




        }
        private bool BeAFutureDate(DateOnly? date)
        {
            return date.HasValue && date > DateOnly.FromDateTime(DateTime.Now);
        }
    }
}

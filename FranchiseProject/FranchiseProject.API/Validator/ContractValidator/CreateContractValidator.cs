using FluentValidation;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace FranchiseProject.API.Validator.ContractValidator
{
    public class CreateContractValidator :AbstractValidator<CreateContractViewModel>
    {
        public CreateContractValidator()
        {



            //RuleFor(x => x.Duration)
            //    .Must(d => new[] { 1, 2, 3, 5, 10 }.Contains(d)) // Year options
            //    .WithMessage("Duration must be one of the following: 1, 2, 3, 5, or 10 years.");

            RuleFor(x => x.Title)
                .MaximumLength(50)
                .WithMessage("Title must be less than 100.")
                .NotNull()
                .WithMessage("Title cannot be null.");
            RuleFor(x => x.StartTime)
                    .NotEmpty().WithMessage("Start Time must not be empty");
            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End Time must not be empty");

            RuleFor(x => x.AgencyId)
                .NotNull()
                .WithMessage("Agency ID cannot be null.");
            RuleFor(x => x.RevenueSharePercentage)
                .NotEmpty().WithMessage("Revenue Share Percentage must not be empty")
                .GreaterThan(0).WithMessage("Revenue Share Percentage must be greater than 0.")
                .LessThan(100).WithMessage("Revenue Share Percentage must be less than 0");
        }
        private bool BeAValidDate(string date)
        {
            return DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private bool BeAPastDate(string date)
        {
            if (DateTime.TryParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate < DateTime.Now;
            }
            return false;
        }
    }
}

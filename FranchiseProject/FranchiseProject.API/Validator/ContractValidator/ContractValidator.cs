using FluentValidation;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace FranchiseProject.API.Validator.ContractValidator
{
    public class ContractValidator :AbstractValidator<CreateContractViewModel>
    {
        public ContractValidator()
        {
            RuleFor(x => x.CCCD).NotEmpty().WithMessage("CCCD cannot be null")
                .Length(12).WithMessage("The CCCD must have exactly 12 digits")
                .Matches(@"^\d{12}$").WithMessage("The CCCD must contain only digits");
            RuleFor(x => x.CCCDdate)
                    .NotEmpty().WithMessage("Date must not be empty")
                    .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("Date must be in DD/MM/YYYY format")
                    .Must(BeAValidDate).WithMessage("Date is not valid")
                    .Must(BeAPastDate).WithMessage("Date must be in the past");
            RuleFor(x => x.CCCDwhere).NotEmpty().WithMessage("CCCDwhere cannot be null");

            RuleFor(x => x.BankNumber).NotEmpty().WithMessage("Bank number cannot be null")
               .Matches(@"^\d+$").WithMessage("Bank number must contain only digits");

            RuleFor(x => x.BankNumber).NotEmpty().WithMessage("Bank cannot be null");

            RuleFor(x => x.Deposit).NotEmpty().WithMessage("Deposit  cannot be null")
                .Matches(@"^\d+$").WithMessage("Deposit must contain only digits");

            RuleFor(x => x.TotalMoney).NotEmpty().WithMessage("Total money cannot be null")
                .Matches(@"^\d+$").WithMessage("Total money must contain only digits");

            RuleFor(x => x.DesignFee).NotEmpty().WithMessage("Design fee cannot be null")
               .Matches(@"^\d+$").WithMessage("Design fee must contain only digits");

            RuleFor(x => x.FranchiseFee).NotEmpty().WithMessage("Franchise fee cannot be null")
               .Matches(@"^\d+$").WithMessage("Franchise fee must contain only digits");

            RuleFor(x => x.RevenueSharePercentage).NotEmpty().WithMessage("RevenueSharePercentage fee cannot be null")
               .Matches(@"^\d+$").WithMessage("RevenueSharePercentage fee must contain only digits")
               .Must(x => int.TryParse(x, out int value) && value < 100)
               .WithMessage("Bank number must be less than 100");

            RuleFor(x => x.Duration)
                .Must(d => new[] { 1, 2, 3, 5, 10 }.Contains(d)) // Year options
                .WithMessage("Duration must be one of the following: 1, 2, 3, 5, or 10 years.");

            RuleFor(x => x.Description)
                .NotNull()
                .WithMessage("Description cannot be null.");

            RuleFor(x => x.AgencyId)
                .NotNull()
                .WithMessage("Agency ID cannot be null.");
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

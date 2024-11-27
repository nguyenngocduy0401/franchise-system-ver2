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
            RuleFor(x => x.Title)
                    .MaximumLength(50)
                    .WithMessage("Tiêu đề không được dài quá 50 ký tự.")
                    .NotNull()
                    .WithMessage("Tiêu đề không được để trống.");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("Thời gian bắt đầu không được để trống.");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("Thời gian kết thúc không được để trống.");

            RuleFor(x => x.AgencyId)
                .NotNull()
                .WithMessage("Mã đại lý không được để trống.");

            RuleFor(x => x.RevenueSharePercentage)
                .NotEmpty()
                .WithMessage("Tỷ lệ chia sẻ doanh thu không được để trống.")
                .GreaterThan(0)
                .WithMessage("Tỷ lệ chia sẻ doanh thu phải lớn hơn 0.")
                .LessThan(100)
                .WithMessage("Tỷ lệ chia sẻ doanh thu phải nhỏ hơn 100.");
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

using FluentValidation;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;

namespace FranchiseProject.API.Validator.AppointmentValidator
{
    public class UpdateAppointmentReportValidator : AbstractValidator<SubmitAppointmentModel>
    {
        public UpdateAppointmentReportValidator()
        {
            RuleFor(x => x.Report.GetTextWithoutHtml())
                .MaximumLength(3000)
                .WithMessage("Nội dung báo cáo không được dài quá 3000 ký tự.");

            RuleFor(x => x.ReportImageURL)
                .MaximumLength(500)
                .WithMessage("Địa chỉ URL hình ảnh báo cáo không được dài quá 500 ký tự.");
        }
    }
}

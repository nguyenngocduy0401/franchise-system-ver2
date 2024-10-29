using FluentValidation;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;

public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentViewModel>
{
    public CreateAssignmentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

        RuleFor(x => x.StartTime)
            .NotNull().WithMessage("Thời gian bắt đầu không được để trống.")
            .LessThan(x => x.EndTime).WithMessage("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");

        RuleFor(x => x.EndTime)
            .NotNull().WithMessage("Thời gian kết thúc không được để trống.")
            .GreaterThan(x => x.StartTime).WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Trạng thái không hợp lệ.");

        RuleFor(x => x.ClassId)
            .NotEmpty().WithMessage("ID lớp không được để trống.")
            .Length(1, 50).WithMessage("ID lớp phải từ 1 đến 50 ký tự.");
    }
}


using FranchiseProject.Domain.Enums;

namespace FranchiseProject.Application.ViewModels.AppointmentViewModels
{
    public class AppointmentDetailViewModel
    {
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public string? Report { get; set; }
        public string? ReportImageURL { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
        public List<AppointmentUserViewModel>? User { get; set; }
    }
}

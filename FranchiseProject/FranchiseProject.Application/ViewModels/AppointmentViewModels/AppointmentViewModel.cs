
using FranchiseProject.Domain.Enums;

namespace FranchiseProject.Application.ViewModels.AppointmentViewModels
{
    public class AppointmentViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
    }
}

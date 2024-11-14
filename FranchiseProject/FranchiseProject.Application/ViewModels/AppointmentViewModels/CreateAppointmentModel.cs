

using FranchiseProject.Domain.Enums;

namespace FranchiseProject.Application.ViewModels.AppointmentViewModels
{
    public class CreateAppointmentModel
    {
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
        public string? Description { get; set; }
        public List<string>? UserId { get; set; }
        public Guid? WorkId { get; set; }
    }
}

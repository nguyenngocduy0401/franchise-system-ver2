

namespace FranchiseProject.Application.ViewModels.AppointmentViewModels
{
    public class UpdateAppointmentModel
    {
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
    }
}

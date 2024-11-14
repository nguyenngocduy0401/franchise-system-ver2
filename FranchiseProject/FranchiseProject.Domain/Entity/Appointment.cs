using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Appointment : BaseEntity
    {
        public string? Title { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public string? Report { get; set; }
        public string? ReportImageURL { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
        public Guid? WorkId { get; set; }
        [ForeignKey("WorkId")]
        public Work? Work { get; set; }
        public virtual ICollection<UserAppointment>? UserAppointments { get; set; }
    }
}

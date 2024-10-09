using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Appointment : BaseEntity
    {
        public string? Title { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public string? Report { get; set; }
        public string? ReportImageURL { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
        public virtual ICollection<AppointmentDetail>? Appointments { get; set; }
    }
}

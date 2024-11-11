using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Work : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public WorkStatusEnum? Status { get; set; }
        public string? Report { get; set; }
        public string? ReportImageURL { get; set; }
        public WorkTypeEnum? Type { get; set; }
        public virtual ICollection<Appointment>? Appointments { get; set; }

    }
}

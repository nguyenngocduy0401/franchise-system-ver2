using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class WorkTemplate : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? StartDaysOffset { get; set; }
        public double? DurationDays { get; set; }
        public WorkLevelEnum? Level { get; set; }
        public WorkTypeEnum? Type { get; set; }
        public virtual ICollection<AppointmentTemplate>? Appointments { get; set; }
    }
}

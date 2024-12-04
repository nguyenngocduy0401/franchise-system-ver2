using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class AppointmentTemplate : BaseEntity
    {
        public string? Title { get; set; }
        public double? StartDaysOffset { get; set; }
        public double? DurationHours { get; set; }
        public string? Description { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
        public Guid? WorkId { get; set; }
        [ForeignKey("WorkId")]
        public WorkTemplate? Work { get; set; }
    }
}

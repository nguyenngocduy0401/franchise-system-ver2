using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Report : BaseEntity
    {
        public string? Description { get; set; }
        public ReportStatusEnum? Status { get; set; }
       public ReportTypeEnum? Type { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }

        public string? RespondedById { get; set; }
    }
}

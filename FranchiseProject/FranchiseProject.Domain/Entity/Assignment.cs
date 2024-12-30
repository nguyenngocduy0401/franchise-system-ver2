using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Assignment : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileURL {get; set;}
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AssigmentTypeEnum Type { get; set; }
        public AssigmentStatusEnum Status { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
        public Guid? AssessmentId { get; set; }
        [ForeignKey("AssessmentId")]
        public Assessment? Assessment { get; set; }
        public virtual ICollection<AssignmentSubmit>? AssignmentSubmits { get; set; }
    }
}

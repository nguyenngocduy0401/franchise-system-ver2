using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Contract : BaseEntity
    {
        public string? Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public string? Description { get; set; }
        public string? TermsAndCondition { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}

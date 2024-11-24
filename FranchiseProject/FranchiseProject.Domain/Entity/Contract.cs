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
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Total { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; }
        public string? DesignFee { get; set; }
        public string? FrachiseFee { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Equipment>? Equipments { get; set; }
    }
}

using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Payment : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? Amount { get; set; }
        public PaymentTypeEnum? Type { get; set; }
        public PaymentStatusEnum? Status { get; set; }
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
        public string ? UserId  { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Equipment:BaseEntity
    {
       // public EquipmentTypeEnum? Type { get; set; }
        public string? EquipmentName { get; set; }
        public string? SerialNumber { get; set; }
        public EquipmentStatusEnum? Status { get; set;  }
        public int? Quantity { get; set; }
        public string? Note { get; set; }
        public double? Price { get; set; }
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
        public ICollection<EquipmentSerialNumberHistory>? SerialNumberHistories { get; set; }
    }
}

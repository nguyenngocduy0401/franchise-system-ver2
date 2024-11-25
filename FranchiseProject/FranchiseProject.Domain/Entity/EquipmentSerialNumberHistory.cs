using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class EquipmentSerialNumberHistory :BaseEntity
    {
        public Guid EquipmentId { get; set; }
        [ForeignKey("EquipmentId")]
        public Equipment? Equipment { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

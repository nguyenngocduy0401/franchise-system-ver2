using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class EquipmentTypePrice : BaseEntity 
    {
        public EquipmentTypeEnum Type { get; set; }
        public double Price { get; set; }
    }
}

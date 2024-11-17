using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.EquipmentViewModels
{
    public class EquipmentRequestViewModel
    {
        public EquipmentTypeEnum Type { get; set; }
        public int Quantity { get; set; }
        public Guid ContractId { get; set; }
        public string? Note { get; set; }
    }
}

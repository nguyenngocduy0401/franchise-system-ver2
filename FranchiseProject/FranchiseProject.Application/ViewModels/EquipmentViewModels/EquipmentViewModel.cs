using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.EquipmentViewModels
{
    public class EquipmentViewModel
    {
        public Guid? Id { get; set; }
        public EquipmentTypeEnum? Type { get; set; }
        public EquipmentStatusEnum? Status { get; set; }
        public int? Quantity { get; set; }
        public string? Note { get; set; }
        public double? Price { get; set; }
       
    }
}

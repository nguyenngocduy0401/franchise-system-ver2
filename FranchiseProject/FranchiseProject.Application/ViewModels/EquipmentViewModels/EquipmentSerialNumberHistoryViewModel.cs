using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.EquipmentViewModels
{
    public class EquipmentSerialNumberHistoryViewModel
    {
       
            public Guid? Id { get; set; }
            public Guid? EquipmentId { get; set; }
            public string? SerialNumber { get; set; }
            public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}

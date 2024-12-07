using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.EquipmentViewModels
{
    public class UpdateReportEquipmentViewModel
    {
        public string? Description { get; set; }
        public List<Guid>? EquipmentIds { get; set; }
    }
}

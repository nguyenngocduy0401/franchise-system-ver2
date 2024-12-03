using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ReportViewModels
{
    public class CreateReportEquipmentViewModel
    {
        public string? Description { get; set; }
        public List<Guid>? EquipmentIds { get; set; }
    }
}

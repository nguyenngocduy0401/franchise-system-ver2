using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.SyllabusViewModels
{
    public class UpdateSyllabusModel
    {
        public string? Description { get; set; }
        public string? StudentTask { get; set; }
        public string? TimeAllocation { get; set; }
        public string? ToolsRequire { get; set; }
        public double Scale { get; set; }
        public double MinAvgMarkToPass { get; set; }
    }
}

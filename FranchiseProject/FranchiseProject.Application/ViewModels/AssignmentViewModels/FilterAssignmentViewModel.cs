using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public  class FilterAssignmentViewModel
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AssigmentStatusEnum Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

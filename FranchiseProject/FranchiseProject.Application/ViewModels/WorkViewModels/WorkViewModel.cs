using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.WorkViewModels
{
    public class WorkViewModel
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public WorkTypeEnum? Type { get; set; }
        public WorkStatusEnum? Status { get; set; }
        public WorkLevelEnum? Level { get; set; }
    }
}

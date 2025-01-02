using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public class StudentAsmViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileURL { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public AssigmentTypeEnum Type { get; set; }
        public Guid? ClassId { get; set; }
        public AsmSubmitViewModel? AsmSubmits { get; set; }
    }
}

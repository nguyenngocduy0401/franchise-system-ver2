using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public class AssignmentViewModel
    {
            public Guid Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? FileURL { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public AssigmentStatusEnum Status { get; set; }
            public Guid? ClassId { get; set; }
            public List<AsmSubmitViewModel>? AsmSubmits { get; set; }
    }
}

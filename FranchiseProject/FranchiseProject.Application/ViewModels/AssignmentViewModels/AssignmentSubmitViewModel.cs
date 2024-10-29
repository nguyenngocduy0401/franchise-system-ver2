using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public class AssignmentSubmitViewModel
    {
        public Guid? AssignmentId { get; set; }
        public string? AssignmentName { get; set; }
        public string? FileSubmitURL { get; set; }
        public DateTime? SubmitDate { get; set; }
    }
}

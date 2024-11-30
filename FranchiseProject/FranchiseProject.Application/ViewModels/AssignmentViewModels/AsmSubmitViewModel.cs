using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssignmentViewModels
{
    public class AsmSubmitViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FileSubmitURL { get; set; }
        public DateTime? SubmitDate { get; set; }
        public double ScoreNumber { get; set; }
    }
}

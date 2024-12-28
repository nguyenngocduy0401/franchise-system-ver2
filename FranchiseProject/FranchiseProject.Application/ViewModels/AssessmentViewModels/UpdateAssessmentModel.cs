using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssessmentViewModels
{
    public class UpdateAssessmentModel
    {
        public int Number { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public string? Content { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public double CompletionCriteria { get; set; }
        //public string? Duration { get; set; }
    }
}

using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssessmentViewModels
{
    public class AssessmentViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public string? Content { get; set; }
        public int Quatity { get; set; }
        public double Weight { get; set; }
        public double CompletionCriteria { get; set; }
        //public string? Duration { get; set; }
        public Guid? CourseId { get; set; }
    }
}

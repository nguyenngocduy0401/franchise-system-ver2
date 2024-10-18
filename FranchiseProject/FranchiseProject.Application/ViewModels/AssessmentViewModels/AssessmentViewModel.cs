using FranchiseProject.Domain.Entity;
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
        public string? Type { get; set; }
        public string? Content { get; set; }
        public int Quatity { get; set; }
        public double Weight { get; set; }
        public double CompletionCriteria { get; set; }
        public string? Method { get; set; }
        public string? Duration { get; set; }
        public string? QuestionType { get; set; }
        public Guid? CourseId { get; set; }
    }
}

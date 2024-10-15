using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Assessment : BaseEntity
    {
        public string? Type { get; set; }
        public string? Content { get; set; }
        public int Quatity { get; set; }
        public double? Weight { get; set; }
        public string? CompletionCriteria { get; set; }
        public string? Method { get; set; }
        public string? Duration { get; set; }
        public string? QuestionType { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}


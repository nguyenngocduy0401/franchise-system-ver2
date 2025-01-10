using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssessmentViewModels
{
    public class AssessmentStudentViewModel
    {
        public AssessmentAttendanceViewModel? AssessmentAttendanceView { get; set; }
        public AssessmentQuizViewModel? AssessmentQuizView { get; set; }
        public AssessmentAssignmentViewModel? AssessmentAssignmentView { get; set; }
        public AssessmentFinalViewModel? AssessmentFinalViewModel { get; set; }
        public string? Certification { get; set; }
        public double MinAvgMarkToPass { get; set; }
        public double AverageScore { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime? TimeCompleted { get; set; }
    }
}

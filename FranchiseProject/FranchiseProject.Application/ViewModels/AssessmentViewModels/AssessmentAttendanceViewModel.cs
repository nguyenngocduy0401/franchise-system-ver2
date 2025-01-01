using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AssessmentViewModels
{
    public class AssessmentAttendanceViewModel
    {
        public Guid Id { get; set; }
        public AssessmentTypeEnum Type { get; set; }
        public string? Content { get; set; }
        public int Quantity { get; set; }
        public double Weight { get; set; }
        public double CompletionCriteria { get; set; }
        public int TotalDaysAttended { get; set; }
        public int AttendedDays { get; set; }
        public double Score { get; set; }
    }
}

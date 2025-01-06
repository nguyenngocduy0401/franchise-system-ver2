using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModels
{
    public   class StudentScheduleDetailModel
    {
        public Guid ScheduleId { get; set; }
        public Guid ClassId { get; set; }
        public Guid SlotId { get; set; }
        public string? Room { get; set; }
        public string? ClassName { get; set; }
        public string? SlotName { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AttendanceStatusEnum? AttendanceStatus { get; set; }
        public bool? Status { get; set; }
        public string? Url { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherId { get; set; }
        public string? LessionName { get; set; }
        public Guid? ChapterId { get; set; }
    }
}

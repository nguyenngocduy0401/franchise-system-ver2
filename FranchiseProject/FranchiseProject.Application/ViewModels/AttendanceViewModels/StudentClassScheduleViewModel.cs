using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AttendanceViewModels
{
    public class StudentClassScheduleViewModel
    {
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? StudentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? URLImage { get; set; }
        public AttendanceStatusEnum? AttendanceStatus { get; set; }
    }
}

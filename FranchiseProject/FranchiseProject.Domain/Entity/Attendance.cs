using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Attendance
    {
        public AttendanceStatusEnum Status { get; set; }
        public Guid? ClassScheduleId { get; set; }
        [ForeignKey("ClassScheduleId")]
        public ClassSchedule? ClassSchedule { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

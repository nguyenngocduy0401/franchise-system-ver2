using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Slot : BaseEntity
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public virtual ICollection<ClassSchedule>? ClassSchedules { get; set; }
    }
}

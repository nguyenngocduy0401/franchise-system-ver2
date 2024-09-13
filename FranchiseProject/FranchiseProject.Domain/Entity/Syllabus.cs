using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Syllabus : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? TimeAllocation { get; set; }
        public string? ToolsRequire { get; set; }
        public double? MinAvgMarkToPass { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class CourseCategory : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Course>? Courses { get; set; } 
    }
}

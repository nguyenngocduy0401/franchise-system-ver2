using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class TempRegistrations :BaseEntity
    {
        public string? UserId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? AgencyId { get; set; }
        public Guid ClassId { get; set; }
    }
}

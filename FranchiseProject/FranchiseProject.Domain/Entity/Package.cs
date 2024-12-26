using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Package : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? NumberOfUsers { get; set; }
        public PackageStatusEnum? Status { get; set; }
        public virtual ICollection<Contract>? Contracts { get; set; }
    }
}

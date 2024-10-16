using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Material : BaseEntity
    {
        public string? URL { get; set; }
        public string? Description { get; set; }
    }
}

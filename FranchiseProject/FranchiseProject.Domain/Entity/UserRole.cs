using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class UserRole : IdentityUserRole<string>
    {
        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}

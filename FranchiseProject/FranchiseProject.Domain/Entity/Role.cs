﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Role : IdentityRole
    {
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}

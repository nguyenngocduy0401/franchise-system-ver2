﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Video : BaseEntity
    {
        public string? Name { get; set; }
        public string? URL { get; set; }
    }
}

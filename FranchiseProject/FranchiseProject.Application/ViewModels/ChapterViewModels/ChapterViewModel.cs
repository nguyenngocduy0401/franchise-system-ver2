﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterViewModels
{
    public class ChapterViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public string? Topic { get; set; }
    }
}

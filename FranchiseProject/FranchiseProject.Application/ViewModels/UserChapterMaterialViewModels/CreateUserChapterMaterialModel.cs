﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserChapterMaterialViewModels
{
    public class CreateUserChapterMaterialModel
    {
        public string? UserId { get; set; }
        public Guid ChapterMaterialId { get; set; }
    }
}

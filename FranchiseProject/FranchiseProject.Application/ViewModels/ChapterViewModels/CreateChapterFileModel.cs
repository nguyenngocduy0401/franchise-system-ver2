﻿using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterViewModels
{
    public class CreateChapterFileModel
    {
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public List<CreateQuestionArrangeModel>? Questions { get; set; }
        public List<CreateChapterMaterialArrangeModel>? ChapterMaterials { get; set; }
    }
}

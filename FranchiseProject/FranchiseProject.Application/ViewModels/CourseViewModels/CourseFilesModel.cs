using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class CourseFilesModel
    {
        public IFormFile? CourseFile { get; set; }
        public IFormFile? QuestionFile { get; set; }
        public IFormFile? ChapterMaterialFile { get; set; }
    }
}


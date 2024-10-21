using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterViewModels
{
    public class CreateChapterModel
    {
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }/*
        public ICollection<Creat>*//*
        public ICollection<Creat>*/
    }
}

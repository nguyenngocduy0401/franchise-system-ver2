using FranchiseProject.Application.ViewModels.UserChapterMaterialViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterMaterialViewModels
{
    public class ChapterMaterialStudentViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public string? Title { get; set; }
        public string? URLFile { get; set; }
        public string? URLVideo { get; set; }
        public string? Description { get; set; }
        public Guid? ChapterId { get; set; }
        public UserChapterMaterialModel? UserChapterMaterials { get; set; }
    }
}

using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterViewModels
{
    public class ChapterDetailViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }
        public ICollection<ChapterMaterialViewModel>? ChapterMaterials { get; set; }
    }
}

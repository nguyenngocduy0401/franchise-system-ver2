using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ChapterMaterialViewModels
{
    public class ChapterMaterialViewModel
    {
        public Guid? Id { get; set; }
        public int Number { get; set; }
        public string? URL { get; set; }
        public string? Description { get; set; }
        public Guid? ChapterId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.UserChapterMaterialViewModels
{
    public class UserChapterMaterialModel
    {
        public int UserId { get; set; }
        public int ChapterMaterialId { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}

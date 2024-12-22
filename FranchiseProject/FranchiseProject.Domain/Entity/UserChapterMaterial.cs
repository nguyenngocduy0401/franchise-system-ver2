using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class UserChapterMaterial
    {
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? ChapterMaterialId { get; set; }

        [ForeignKey("ChapterMaterialId")]
        public ChapterMaterial? ChapterMaterial { get; set; }
    }
}

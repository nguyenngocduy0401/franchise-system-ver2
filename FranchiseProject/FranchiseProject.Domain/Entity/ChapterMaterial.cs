using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class ChapterMaterial : BaseEntity
    {
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? URL { get; set; }
        public string? Description { get; set; }
        public Guid? ChapterId { get; set; }
        [ForeignKey("ChapterId")]
        public Chapter? Chapter { get; set; }
    }
}


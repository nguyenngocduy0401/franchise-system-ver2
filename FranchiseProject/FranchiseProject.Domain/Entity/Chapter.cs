using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Chapter : BaseEntity
    {
        public int Number { get; set; }
        public string? Topic { get; set; }
        public string? Description { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
        public virtual ICollection<ChapterMaterial>? ChapterMaterials { get; set; }
    }
}

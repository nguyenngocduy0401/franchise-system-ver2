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
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FileURL { get; set; }
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
    }
}

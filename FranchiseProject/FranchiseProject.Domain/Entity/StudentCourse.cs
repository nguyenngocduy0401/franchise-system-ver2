using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class StudentCourse
    {
        public Guid? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
        public Guid? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
       
    }
}

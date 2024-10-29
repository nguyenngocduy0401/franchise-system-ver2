using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Course :BaseEntity
    {
        public string? Name {get; set;}
        public string? Description {get; set;}
        public string? URLImage {get; set;}
        public int? NumberOfLession {get; set;}
        public int? Price { get; set; }
        public string? Code { get; set; }
        public int? Version { get; set; }
        public CourseStatusEnum Status {get; set;}
        public Guid? SyllabusId {get; set;}
        [ForeignKey("SyllabusId")]
        public Syllabus? Syllabus {get; set;}
        public Guid? CourseCategoryId { get; set;}
        [ForeignKey("CourseCategoryId")]
        public CourseCategory? CourseCategory { get; set;}
        public virtual ICollection<RegisterCourse>? RegisterCourses { get; set; }
        public virtual ICollection<Session>? Sessions {get; set;}
        public virtual ICollection<Chapter>? Chapters {get; set;}
        public virtual ICollection<Report>? Reports {get; set;}
        public virtual ICollection<Class>? Classes {get; set;}
        public virtual ICollection<Assessment>? Assessments {get; set;}
        public virtual ICollection<CourseMaterial>? CourseMaterials { get; set;}
    }
}

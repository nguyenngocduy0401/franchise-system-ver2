using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class CourseDetailViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? URLImage { get; set; }
        public string? NumberOfSession { get; set; }
        public CourseStatusEnum Status { get; set; }
        public Guid? SyllabusId { get; set; }
        public Syllabus? Syllabus { get; set; }
        public Guid? CourseCategoryId { get; set; }
        public CourseCategory? CourseCategory { get; set; }
        public virtual ICollection<Session>? Sessions { get; set; }
        public virtual ICollection<Chapter>? Chapters { get; set; }
        public virtual ICollection<Assessment>? Assessments { get; set; }
        public virtual ICollection<Material>? Materials { get; set; }
    }
}

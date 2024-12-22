using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.CourseViewModels
{
    public class CourseStudentViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? URLImage { get; set; }
        public int NumberOfLession { get; set; }
        public int Price { get; set; }
        public string? Code { get; set; }
        public int Version { get; set; }
        public CourseStatusEnum Status { get; set; }
        public Guid? SyllabusId { get; set; }
        public SyllabusViewModel? Syllabus { get; set; }
        public Guid? CourseCategoryId { get; set; }
        public CourseCategoryViewModel? CourseCategory { get; set; }
        public virtual ICollection<ChapterStudentViewModel>? Chapters { get; set; }
        public virtual ICollection<AssessmentViewModel>? Assessments { get; set; }
        public virtual ICollection<CourseMaterialViewModel>? CourseMaterials { get; set; }
    }
}

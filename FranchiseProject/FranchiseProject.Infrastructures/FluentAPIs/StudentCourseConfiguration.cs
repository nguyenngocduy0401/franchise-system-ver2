using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.FluentAPIs
{
    public class StudentCourseConfiguration : IEntityTypeConfiguration<StudentCourse>
    {
        public void Configure(EntityTypeBuilder<StudentCourse> builder)
        {
            builder.HasKey(x => new { x.CourseId, x.StudentId });
            builder.HasOne(a => a.Student)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(a => a.StudentId);
            builder.HasOne(a => a.Course)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(a => a.CourseId);
        }
    }
}

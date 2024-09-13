using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
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
            builder.HasKey(x => new { x.CourseId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Course)
                .WithMany(a => a.StudentCourses)
                .HasForeignKey(a => a.CourseId);
        }
    }
}

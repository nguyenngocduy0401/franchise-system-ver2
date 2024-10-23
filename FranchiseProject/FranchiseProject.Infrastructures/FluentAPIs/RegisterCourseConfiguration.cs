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
    public class RegisterCourseConfiguration : IEntityTypeConfiguration<RegisterCourse>
    {
        public void Configure(EntityTypeBuilder<RegisterCourse> builder)
        {
            builder.HasKey(x => new { x.CourseId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.RegisterCourses)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Course)
                .WithMany(a => a.RegisterCourses)
                .HasForeignKey(a => a.CourseId);
        }
    }
}

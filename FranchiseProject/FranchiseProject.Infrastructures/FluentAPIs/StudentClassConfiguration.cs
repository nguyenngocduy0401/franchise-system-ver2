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
    public class StudentClassConfiguration : IEntityTypeConfiguration<StudentClass>
    {
        public void Configure(EntityTypeBuilder<StudentClass> builder)
        {
            builder.HasKey(x => new { x.ClassId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.StudentClasses)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Class)
                .WithMany(a => a.StudentClasses)
                .HasForeignKey(a => a.ClassId);
        }
    }
}

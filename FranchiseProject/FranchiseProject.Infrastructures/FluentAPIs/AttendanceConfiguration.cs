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
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasKey(x => new { x.ClassScheduleId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.Attendances)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.ClassSchedule)
                .WithMany(a => a.Attendances)
                .HasForeignKey(a => a.ClassScheduleId);
        }
    }
}
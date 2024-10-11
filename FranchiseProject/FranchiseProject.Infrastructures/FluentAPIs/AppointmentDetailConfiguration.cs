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
    public class AppointmentDetailConfiguration : IEntityTypeConfiguration<AppointmentDetail>
    {
        public void Configure(EntityTypeBuilder<AppointmentDetail> builder)
        {
            builder.HasKey(x => new { x.AppointmentId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.AppointmentDetails)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Appointment)
                .WithMany(a => a.Appointments)
                .HasForeignKey(a => a.AppointmentId);
        }
    }
}

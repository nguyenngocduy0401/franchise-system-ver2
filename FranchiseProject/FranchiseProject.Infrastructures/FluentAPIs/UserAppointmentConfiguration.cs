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
    public class UserAppointmentConfiguration : IEntityTypeConfiguration<UserAppointment>
    {
        public void Configure(EntityTypeBuilder<UserAppointment> builder)
        {
            builder.HasKey(x => new { x.AppointmentId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.UserAppointments)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Appointment)
                .WithMany(a => a.UserAppointments)
                .HasForeignKey(a => a.AppointmentId);
        }
    }
}

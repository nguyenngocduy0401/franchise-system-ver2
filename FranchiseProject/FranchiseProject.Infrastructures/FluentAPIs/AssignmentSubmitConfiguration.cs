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
    public class AssignmentSubmitConfiguration : IEntityTypeConfiguration<AssignmentSubmit>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmit> builder)
        {
            builder.HasKey(x => new { x.AssignmentId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a. AssignmentSubmits)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Assignment)
                .WithMany(a => a.AssignmentSubmits)
                .HasForeignKey(a => a.AssignmentId);
        }
    }
}

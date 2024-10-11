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
    public class WorkDetailConfiguration : IEntityTypeConfiguration<WorkDetail>
    {
        public void Configure(EntityTypeBuilder<WorkDetail> builder)
        {
            builder.HasKey(x => new { x.WorkId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.WorkDetails)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Work)
                .WithMany(a => a.WorkDetails)
                .HasForeignKey(a => a.WorkId);
        }
    }
}

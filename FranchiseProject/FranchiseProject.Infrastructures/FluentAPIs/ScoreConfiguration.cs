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
    public class ScoreConfiguration : IEntityTypeConfiguration<Score>
    {
        public void Configure(EntityTypeBuilder<Score> builder)
        {
            builder.HasKey(x => new { x.QuizId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.Scores)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.Quiz)
                .WithMany(a => a.Scores)
                .HasForeignKey(a => a.QuizId);
        }
    }
}

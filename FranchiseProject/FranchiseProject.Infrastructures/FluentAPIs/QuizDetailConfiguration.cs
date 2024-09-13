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
    public class QuizDetailConfiguration : IEntityTypeConfiguration<QuizDetail>
    {
        public void Configure(EntityTypeBuilder<QuizDetail> builder)
        {
            builder.HasKey(x => new { x.QuizId, x.QuestionId});
            builder.HasOne(a => a.Question)
                .WithMany(a => a.QuizDetails)
                .HasForeignKey(a => a.QuestionId);
            builder.HasOne(a => a.Quiz)
                .WithMany(a => a.QuizDetails)
                .HasForeignKey(a => a.QuizId);
        }
    }
}

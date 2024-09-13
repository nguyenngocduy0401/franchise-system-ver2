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
    public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.HasKey(x => new { x.QuestionOptionId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.StudentAnswers)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.QuestionOptions)
                .WithMany(a => a.StudentAnswers)
                .HasForeignKey(a => a.QuestionOptionId);
        }
    }
}


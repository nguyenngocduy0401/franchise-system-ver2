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
    public class FeedbackAnswerConfiguration : IEntityTypeConfiguration<FeedbackAnswer>
    {
        public void Configure(EntityTypeBuilder<FeedbackAnswer> builder)
        {
            builder.HasIndex(a => new { a.UserId, a.FeedbackOptionId}).IsUnique();
            builder.HasIndex(a => new { a.UserId, a.FeedbackId}).IsUnique();
        }
    }
}

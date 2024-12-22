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

    public class UserChapterMaterialConfiguration : IEntityTypeConfiguration<UserChapterMaterial>
    {
        public void Configure(EntityTypeBuilder<UserChapterMaterial> builder)
        {
            builder.HasKey(x => new { x.ChapterMaterialId, x.UserId });
            builder.HasOne(a => a.User)
                .WithMany(a => a.UserChapterMaterials)
                .HasForeignKey(a => a.UserId);
            builder.HasOne(a => a.ChapterMaterial)
                .WithMany(a => a.UserChapterMaterials)
                .HasForeignKey(a => a.ChapterMaterialId);
        }
    }

}

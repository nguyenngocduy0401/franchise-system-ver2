using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FranchiseProject.Application.Interfaces;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using FranchiseProject.Domain.Enums;
using FranchiseProject.Application.Services;

namespace FranchiseProject.Infrastructures.FluentAPIs
{
    public class WorkTemplateConfiguration : IEntityTypeConfiguration<WorkTemplate>
    {
        public void Configure(EntityTypeBuilder<WorkTemplate> builder)
        {

        }
    }
}

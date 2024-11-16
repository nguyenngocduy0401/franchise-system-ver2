using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DocumentViewModel
{
    public class DocumentViewModel
    {
        public string? Title { get; set; }
        public string? URLFile { get; set; }
        public Guid? AgencyId { get; set; }
        
    }
}

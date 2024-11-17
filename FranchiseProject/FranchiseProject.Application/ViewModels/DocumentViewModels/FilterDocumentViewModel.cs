using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.DocumentViewModels
{
    public class FilterDocumentViewModel
    {
        public Guid? AgencyId { get; set; }
        public DocumentType? Type { get; set; }
        public DocumentStatus Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

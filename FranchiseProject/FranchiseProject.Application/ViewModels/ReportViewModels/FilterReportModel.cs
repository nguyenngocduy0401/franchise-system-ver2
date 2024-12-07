using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ReportViewModels
{
    public class FilterReportModel
    {
        public Guid? AgencyId { get; set; }
        public ReportStatusEnum? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public ReportTypeEnum? ReportType { get; set; }
   
    
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 

    }
}

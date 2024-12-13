using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class FilterContractPaymentViewModel
    {
      
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ContractCode { get; set; }
        public Guid? AgencyId { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class UpdateContractViewModel
    {
        public string? Title { get; set; }     //
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double? DepositPercentage { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; } //
    
    }
}

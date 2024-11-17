using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class CreateContractViewModel
    {
        public string? Title { get; set; }     //
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public double? RevenueSharePercentage { get; set; } //
        public string? AgencyId { get; set; }
    }
}

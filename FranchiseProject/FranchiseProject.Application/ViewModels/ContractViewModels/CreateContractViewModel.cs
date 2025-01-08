using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class CreateContractViewModel
    {
        public string? Title { get; set; }     //không quá 25 kí tự
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ContractDocumentImageURL { get; set; }// không null
        public double? DepositPercentage { get; set; }
        public double? RevenueSharePercentage { get; set; } //lớn hơn 0 bé hon 100
       
        public string? AgencyId { get; set; }
        public Guid? PackageId { get; set; }
    }
}
    
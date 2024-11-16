using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class CreateContractViewModel
    {
        public string? Title { get; set; }
        public string? CCCD { get; set; } //
        public string? CCCDdate { get; set; }  //
        public string? CCCDwhere { get; set; } //
        public string? BankNumber { get; set; }       //
        public string? Bank { get; set; }      //
        public string? TotalMoney { get; set; }    //
        public string? Deposit { get; set; }       //
        public string? DesignFee { get; set; }     //
        public string? FranchiseFee { get; set; }   //
        public string? RevenueSharePercentage { get; set; } //
        public int Duration { get; set; }//
        public string? Description { get; set; }
        public string? AgencyId { get; set; }
    }
}

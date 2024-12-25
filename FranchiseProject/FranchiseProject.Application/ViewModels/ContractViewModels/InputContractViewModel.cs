using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModels
{
    public class InputContractViewModel
    {
        public string? ContractCode { get; set; }
        public double? TotalMoney { get; set; }
        public double? Deposit { get; set; }
        public double? FranchiseFee { get; set;  }
        public double? Druration { get; set; }
        public double? Percent { get; set; }
        public string? Address { get; set; }
    }
}

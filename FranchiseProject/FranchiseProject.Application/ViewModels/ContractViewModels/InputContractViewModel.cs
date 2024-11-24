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
        public double? TotalMoney { get; set; }
     //   public double? Deposit { get; set; }
        public double? DesignFee { get; set; }
        public double? FranchiseFee { get; set;  }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModel
{
    public class UpdateContractViewModel
    {
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public string? TermsAndCondition { get; set; }
    }
}

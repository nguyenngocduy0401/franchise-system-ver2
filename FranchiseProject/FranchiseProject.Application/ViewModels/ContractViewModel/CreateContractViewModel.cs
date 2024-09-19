using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModel
{
    public class CreateContractViewModel
    {
        public string? Title { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string? PositionImageURL { get; set; }
        public string? Description { get; set; }
        public string? TermsAndCondition { get; set; }
        public string? AgencyId { get; set; }
    }
}

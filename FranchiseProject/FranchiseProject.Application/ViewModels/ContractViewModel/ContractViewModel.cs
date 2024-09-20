using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ContractViewModel
{
    public  class ContractViewModel
    {
        public Guid? Id { get; set; }
        public string? Title {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string? ContractDocumentImageURL { get; set; }
        public string? PositionImageURL { get; set; }
        public string? Description { get; set; }
        public string? TermsAndCondition { get; set; }
        public string? AgencyName { get; set; }
    }
}

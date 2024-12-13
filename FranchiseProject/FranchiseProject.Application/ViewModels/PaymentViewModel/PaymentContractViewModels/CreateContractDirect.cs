using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels
{
    public class CreateContractDirect
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
       public  string? ImageUrl { get; set; }
        public Guid  AgencyId { get; set; }
    }
}

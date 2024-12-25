using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.VnPayViewModels
{
    public class CreateAgencyVNPayInfoViewModel
    {
        public Guid AgencyId { get; set; }
        public string VnpayTmnCode { get; set; }
        public string VnpayHashSecret { get; set; }
    }
}

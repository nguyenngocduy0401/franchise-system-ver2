using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AgenciesViewModels
{
	public class AgencyAddressViewModel
	{
		public Guid? Id { get; set; }
		public string? FullAddress { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
    }
}

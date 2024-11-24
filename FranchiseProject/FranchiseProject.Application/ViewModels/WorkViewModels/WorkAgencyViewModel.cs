using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.WorkViewModels
{
    public class WorkAgencyViewModel
    {
        public IEnumerable<WorkViewModel>? Work { get; set; }
        public AgencyStatusEnum? AgencyStatus { get; set; }
    }
}

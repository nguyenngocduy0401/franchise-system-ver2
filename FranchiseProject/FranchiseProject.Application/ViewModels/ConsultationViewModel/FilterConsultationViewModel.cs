using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AgencyViewModel
{
    public  class FilterConsultationViewModel
    {
      public  ConsultationStatusEnum? Status { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

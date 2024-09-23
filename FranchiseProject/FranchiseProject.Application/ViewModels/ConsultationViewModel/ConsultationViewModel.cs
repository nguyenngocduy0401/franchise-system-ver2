using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AgencyViewModel
{
    public  class ConsultationViewModel
    {
        public Guid Id { get; set; }
        public string? CusomterName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public ConsultationStatusEnum Status { get; set; }
         public string? ConsultantUserName { get; set; }
    }
}

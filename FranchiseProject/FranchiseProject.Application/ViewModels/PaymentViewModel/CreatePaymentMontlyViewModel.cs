using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public  class CreatePaymentMontlyViewModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Amount { get; set; }
        public PaymentTypeEnum? Type { get; set; }
        public PaymentMethodEnum? Method { get; set; }
        public PaymentStatus? Status { get; set; }
        public Guid? ContractId { get; set; }
        public Guid? AgencyId { get; set; }
        public string? PaymentUrl { get; set; }
    }
}

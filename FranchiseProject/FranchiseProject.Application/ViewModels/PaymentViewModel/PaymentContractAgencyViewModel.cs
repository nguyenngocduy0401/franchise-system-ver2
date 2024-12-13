using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public class PaymentContractAgencyViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Amount { get; set; }
        public PaymentMethodEnum? Method { get; set; }
        public PaymentStatus? Status { get; set; }
        public string? ImageURL { get; set; }

        public Guid? ContractId { get; set; }
        public string? ContractCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AgencyName { get; set; }
    }
}

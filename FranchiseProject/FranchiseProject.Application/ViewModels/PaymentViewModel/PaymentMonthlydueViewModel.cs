using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public class PaymentMonthlydueViewModel
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Amount { get; set; }
        public PaymentStatus? Status { get; set; }
        public Guid? AgencyId { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime? PaidDate { get; set; }

    }
}

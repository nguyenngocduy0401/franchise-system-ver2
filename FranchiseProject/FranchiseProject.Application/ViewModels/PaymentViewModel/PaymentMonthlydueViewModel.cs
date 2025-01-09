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
        public double? Amount { get; set; }// Số tiền cần trả 
        public PaymentStatus? Status { get; set; }
        public Guid? AgencyId { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? CreattionDate { get; set; }
        public double? Revenue { get; set; }//Doanh thu gộp
        public double? RevenueSharePercentage { get; set; }
        public double? ActualProfits { get; set; }//Lợi nhuận thực tế 
        public double? Refunds { get; set; }//Hoàn tiền

    }
}

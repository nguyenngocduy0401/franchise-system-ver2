using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public class CreateRefundPaymentViewModel
    {
        public Guid RegisterCourseId  { get; set; }
        public string? RefundReason { get; set; }
        public string? ImageUrl { get; set; }
    }
}

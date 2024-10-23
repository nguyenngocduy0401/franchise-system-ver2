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
    public class PaymentStudentViewModel
    {
        public string? Title { get; set; }
        public string?StudentName { get; set; }
        public string? Description { get; set; }
        public int? Amount { get; set; }
        public StudentPaymentStatusEnum? Status { get; set; }
    }
}

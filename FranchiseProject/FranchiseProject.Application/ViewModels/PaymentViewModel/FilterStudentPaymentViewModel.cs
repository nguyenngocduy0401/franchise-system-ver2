using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public class FilterStudentPaymentViewModel
    {
       
        public StudentPaymentStatusEnum? Status { get; set; }
        public string? StudentName { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}

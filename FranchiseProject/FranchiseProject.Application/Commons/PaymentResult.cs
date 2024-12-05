using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Commons
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string OrderId { get; set; }
        public string Message { get; set; }
    }
}

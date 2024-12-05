using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class VnPayConfig
    {
        public string? TmnCode { get; set; }
        public string? HashSecret { get; set; }
        public string? PaymentUrl { get; set; }
        public string? ReturnUrl { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Notification :BaseEntity
    {
        public string? Message { get; set; }
        public string? SenderId { get; set; }
        public User? Sender { get; set; }
        public string? ReceiverId { get; set; }
        public User? Receiver { get; set; }
        public bool IsRead { get; set; }
    }
}

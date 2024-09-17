using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public  class FranchiseRegistrationRequests
    {
        public Guid Id {  get; set; }
        public string? CusomterName { get; set; }
        public string? Email {  get; set; }
        public int? PhoneNumber {  get; set; }
        public string? Address {  get; set; }
        public FranchiseRegistrationStatusEnum Status { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? ConsultantUserName { get; set; }
    }
}

using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public  class Consultation :BaseEntity
    {

        public string? CusomterName { get; set; }
        public string? Email {  get; set; }
        public string? PhoneNumber {  get; set; }
        public ConsultationStatusEnum Status { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
       
    }
}

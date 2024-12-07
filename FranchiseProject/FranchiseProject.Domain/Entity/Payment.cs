using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class Payment : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Amount { get; set; }
        public PaymentTypeEnum? Type { get; set; }
        public PaymentMethodEnum? Method { get; set; }
        public PaymentStatus? Status { get; set; }
        public string? ImageURL { get; set; }
        public Guid? ContractId { get; set; }
        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }
        public string ? UserId  { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? RegisterCourseId { get; set; }
        [ForeignKey("RegisterCourseId")]
        public RegisterCourse? RegisterCourse { get; set; }

    }
}

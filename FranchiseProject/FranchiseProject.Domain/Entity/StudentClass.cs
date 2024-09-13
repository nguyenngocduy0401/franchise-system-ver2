using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class StudentClass
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public StudentClassStatusEnum Status { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
    }
}

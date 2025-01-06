using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class ClassRoom
    {
     
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
        public string? Certification { get; set; }
        public ClassRoomEnumStatus? Status { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get;set; }
    }
}

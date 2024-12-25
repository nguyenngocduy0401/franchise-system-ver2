using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public class ClassSchedule : BaseEntity
    {
        public DateTime? Date { get; set; }
        public Guid? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
        public Guid? SlotId { get; set; }
        [ForeignKey("SlotId")]
        public Slot? Slot { get; set; }
        public string? Room { get; set; }
        public string? Url { get; set; }
        public virtual ICollection<Attendance>? Attendances { get; set; }
        public bool? Status { get; set; }//1 là La da diem danh 0, là chuaw
      }
}

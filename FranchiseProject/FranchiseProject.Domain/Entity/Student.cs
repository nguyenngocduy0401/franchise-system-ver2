using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Domain.Entity
{
    public  class Student :BaseEntity
    {
        public string? StudentName {  get; set; }
        public DateOnly? DateOfBirth {  get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email {  get; set; }
        public string? Address {  get; set; }
       public StudentStatusEnum? Status { get; set; }
        public Guid? AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public Agency? Agency { get; set; }
   
        public virtual ICollection<StudentCourse>? StudentCourses { get; set; }
        public virtual ICollection<Payments>? Payments {  get; set; } 
    }
}

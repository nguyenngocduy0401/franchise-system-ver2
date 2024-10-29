using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.PaymentViewModel
{
    public  class CreateStudentPaymentViewModel
    {
      
        public string UserId { get; set; }
        public string? CourseId {  get; set; }
        public string Title { get; set; }
  //      public string StudentName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        
      
    }
}


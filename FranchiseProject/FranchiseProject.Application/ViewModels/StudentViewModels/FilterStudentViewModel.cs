using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.StudentViewModel
{
    public class FilterStudentViewModel
    {
        StudentPaymentStatusEnum? StatusPayment {  get; set; }
        public StudentStatusEnum? Status {  get; set; } 
        public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

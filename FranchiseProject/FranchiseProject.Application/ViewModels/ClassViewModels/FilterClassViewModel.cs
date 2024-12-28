using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.ClassViewModel
{
    public class FilterClassViewModel
    {
        public string? DayOfWeek { get; set; }
        public Guid? SlotId { get; set; }
        public string? Name { get; set; }
        public ClassStatusEnum? Status {  get; set; }     
        public string? CourseId { get; set; }
    //    public bool? IsDeleted { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

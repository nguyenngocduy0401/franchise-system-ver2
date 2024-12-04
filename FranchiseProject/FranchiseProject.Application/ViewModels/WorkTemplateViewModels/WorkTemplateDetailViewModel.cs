using FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.WorkTemplateViewModels
{
    public class WorkTemplateDetailViewModel
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? StartDaysOffset { get; set; }
        public double? DurationDays { get; set; }
        public WorkLevelEnum? Level { get; set; }
        public WorkTypeEnum? Type { get; set; }
        public virtual ICollection<AppointmentTemViewModel>? Appointments { get; set; }
    }
}

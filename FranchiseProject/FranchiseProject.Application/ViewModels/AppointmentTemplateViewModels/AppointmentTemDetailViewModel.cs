using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels
{
    public class AppointmentTemDetailViewModel
    {
        public string? Title { get; set; }
        public double? StartDaysOffset { get; set; }
        public double? DurationHours { get; set; }
        public string? Description { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
        public AppointmentTypeEnum? Type { get; set; }
    }
}

using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels
{
    public class AppointmentTemViewModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public double? StartDaysOffset { get; set; }
        public double? DurationHours { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
    }
}

using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.ViewModels.AppointmentViewModels
{
    public class SubmitAppointmentModel
    {
        public string? Report { get; set; }
        public string? ReportImageURL { get; set; }
        public AppointmentStatusEnum? Status { get; set; }
    }
}

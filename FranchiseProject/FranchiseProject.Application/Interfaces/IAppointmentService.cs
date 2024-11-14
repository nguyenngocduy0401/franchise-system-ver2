using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAppointmentService
    {
        ApiResponse<bool> CheckAppointmentAvailable(Appointment appointment);
        Task<ApiResponse<bool>> CreateAppointmentAsync(CreateAppointmentModel createAppointmentModel);
    }
}

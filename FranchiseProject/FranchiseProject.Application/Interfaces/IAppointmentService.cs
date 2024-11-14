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
        Task<ApiResponse<bool>> DeleteAppointmentAsync(Guid id);
        Task<ApiResponse<AppointmentDetailViewModel>> GetAppointmentDetailByIdAsync(Guid id);
        Task<ApiResponse<bool>> UpdateAppointmentAsync(Guid id, UpdateAppointmentModel updateAppointmentModel);
    }
}

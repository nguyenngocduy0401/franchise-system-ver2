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
        Task<ApiResponse<bool>> SubmitAppointmentReportAsync(Guid id, SubmitAppointmentModel submitAppointmentModel);
        ApiResponse<bool> CheckAppointmentAvailable(Appointment appointment);
        Task<ApiResponse<bool>> CreateAppointmentAsync(CreateAppointmentModel createAppointmentModel);
        Task<ApiResponse<bool>> DeleteAppointmentAsync(Guid id);
        Task<ApiResponse<AppointmentDetailViewModel>> GetAppointmentDetailByIdAsync(Guid id);
        Task<ApiResponse<bool>> UpdateAppointmentAsync(Guid id, UpdateAppointmentModel updateAppointmentModel);
        Task<ApiResponse<IEnumerable<AppointmentViewModel>>> GetScheduleByLoginAsync(FilterScheduleAppointmentViewModel search);
        Task<ApiResponse<IEnumerable<AppointmentViewModel>>> GetScheduleAgencyByLoginAsync(FilterScheduleAppointmentViewModel search);
    }
}

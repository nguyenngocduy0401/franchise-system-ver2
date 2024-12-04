using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IAppointmentTemplateService
    {
        Task<ApiResponse<List<AppointmentTemViewModel>>> GetAllAppointmentTemplateByWorkIdAsync(Guid workId);
        Task<ApiResponse<bool>> UpdateAppointmentTemplateAsync(Guid appointmentTemplateId, UpdateAppointmentTemplateModel updateAppointmentTemplateModel);
        Task<ApiResponse<bool>> CreateAppointmentTemplateAsync(CreateAppointmentTemplateModel createAppointmentTemplateModel);
        Task<ApiResponse<bool>> DeleteAppointmentTemplateByIdAsync(Guid appointmentTemplateId);
    }
}

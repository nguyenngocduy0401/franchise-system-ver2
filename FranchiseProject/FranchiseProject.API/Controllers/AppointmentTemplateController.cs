using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/appointment-templates")]
    [ApiController]
    public class AppointmentTemplateController : ControllerBase
    {
       private readonly IAppointmentTemplateService _appointmentTemplateService;
        public AppointmentTemplateController(IAppointmentTemplateService appointmentTemplateService)
        {
            _appointmentTemplateService = appointmentTemplateService;
        }
        [SwaggerOperation(Summary = "tạo lịch hẹn của công việc mẫu {Authorize = Admin}")]
        [Authorize(Roles = AppRole.Admin)]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateAppointmentTemplateAsync(CreateAppointmentTemplateModel createAppointmentTemplateModel)
        {
            return await _appointmentTemplateService.CreateAppointmentTemplateAsync(createAppointmentTemplateModel);
        }
        [SwaggerOperation(Summary = "cập nhật lịch hẹn của công việc mẫu {Authorize = Admin}")]
        [Authorize(Roles = AppRole.Admin)]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateAppointmentTemplateAsync(Guid id, UpdateAppointmentTemplateModel updateAppointmentTemplateModel)
        {
            return await _appointmentTemplateService.UpdateAppointmentTemplateAsync(id, updateAppointmentTemplateModel);
        }
        [SwaggerOperation(Summary = "xóa lịch hẹn của công việc mẫu {Authorize = Admin}")]
        [Authorize(Roles = AppRole.Admin)]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteAppointmentTemplateByIdAsync(Guid id)
        {
            return await _appointmentTemplateService.DeleteAppointmentTemplateByIdAsync(id);
        }
    }
}

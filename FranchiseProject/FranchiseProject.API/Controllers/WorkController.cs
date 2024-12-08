using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/works")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly IWorkService _workService;
        public WorkController(IWorkService workService)
        {
            _workService = workService;
        }
        [SwaggerOperation(Summary = "filter công việc có thể lấy tất cả hoặc lấy cho 1 agency cụ thể {Authorize = Manager}")]
        [HttpGet()]
        public async Task<ApiResponse<Pagination<WorkViewModel>>> FilterWorkAsync([FromQuery]FilterWorkModel filterWorkModel)
        {
            return await _workService.FilterWorkAsync(filterWorkModel);
        }
        [SwaggerOperation(Summary = "lấy chi tiết công việc bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<WorkDetailViewModel>> GetWorkDetailByIdAsync(Guid id)
        {
            return await _workService.GetWorkDetailByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "tạo công việc{Authorize = Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateWorkAsync(CreateWorkModel createWorkModel)
        {
            return await _workService.CreateWorkAsync(createWorkModel);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật công việc{Authorize = Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateWorkAsync(Guid id, UpdateWorkModel updateWorkModel)
        {
            return await _workService.UpdateWorkAsync(id, updateWorkModel);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "xóa công việc{Authorize = Manager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteWorkAsync(Guid id)
        {
            return await _workService.DeleteWorkByIdAsync(id);
        }
        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "cập nhật trạng thái công việc{Authorize = Manager}")]
        [HttpPut("~/manager/api/v1/works/{id}")]
        public async Task<ApiResponse<bool>> UpdateStatusWorkByIdAsync(Guid id, WorkStatusEnum status)
        {
            return await _workService.UpdateStatusWorkByIdAsync(id,status);
        }
        [Authorize(Roles = AppRole.Manager + "," +
            AppRole.SystemConsultant + "," + AppRole.SystemTechnician + ","
            + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "cập nhật trạng thái công việc {Authorize = Manager, SystenConsultant, SystemTechniciaan, SystemInstructor}")]
        [HttpPut("~/staff/api/v1/works/{id}/status")]
        public async Task<ApiResponse<bool>> UpdateWorkStatusSubmitByStaffAsync(Guid id, WorkStatusSubmitEnum workStatusSubmitEnum)
        {
            return await _workService.UpdateWorkStatusSubmitByStaffAsync(id, workStatusSubmitEnum);
        }
        [Authorize(Roles = AppRole.Manager + "," +
            AppRole.SystemConsultant + "," + AppRole.SystemTechnician + ","
            + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "nộp báo cáo công việc {Authorize = Manager, SystenConsultant, SystemTechniciaan, SystemInstructor}")]
        [HttpPut("~/staff/api/v1/works/{id}")]
        public async Task<ApiResponse<bool>> UpdateWorkByStaffAsync(Guid id,UpdateWorkByStaffModel updateWorkByStaffModel)
        {
            return await _workService.UpdateWorkByStaffAsync(id, updateWorkByStaffModel);
        }
        [Authorize(Roles = AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "Lấy công việc cho agency {Authorize = AgencyManager}")]
        [HttpGet("~/agency/api/v1/works")]
        public async Task<ApiResponse<Pagination<WorkViewModel>>> UpdateWorkByStaffAsync([FromQuery] FilterWorkByLoginModel filterWorkByLoginModel)
        {
            return await _workService.GetWorksAgencyAsync(filterWorkByLoginModel);
        }

        [Authorize(Roles = AppRole.AgencyManager)]
        [HttpPut("~/agency-manager/api/v1/works/{id}")]
        public async Task<ApiResponse<bool>> UploadFileByAgencyManager(Guid id, string fileURL)
        {
            return await _workService.UploadFileByAgencyManager(id, fileURL);
        }
	}
}

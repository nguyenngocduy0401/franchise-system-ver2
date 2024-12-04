using DocumentFormat.OpenXml.Spreadsheet;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/work-templates")]
    [ApiController]
    public class WorkTemplateController : ControllerBase
    {
        private IWorkTemplateService _workTemplateService;
        public WorkTemplateController(IWorkTemplateService workTemplateService)
        {
            _workTemplateService = workTemplateService;
        }
        [SwaggerOperation(Summary = "Lấy tất cả công việc mẫu {Authorize = Admin}")]
        [HttpGet()]
        public async Task<ApiResponse<List<WorkTemplateViewModel>>> GetAllWorkTemplateAsync()
        {
            return await _workTemplateService.GetAllWorkTemplateAsync();
        }
        [SwaggerOperation(Summary = "Lấy công việc mẫu bằng id {Authorize = Admin}")]
        [HttpGet("id")]
        public async Task<ApiResponse<WorkTemplateDetailViewModel>> GetWorkTemplateDetailByIdAsync(Guid id)
        {
            return await _workTemplateService.GetWorkTemplateDetailByIdAsync(id);
        }
        [SwaggerOperation(Summary = "Tạo công việc mẫu {Authorize = Admin}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateWorkTemplateAsync(CreateWorkTemplateModel createWorkTemplateModel)
        {
            return await _workTemplateService.CreateWorkTemplateAsync(createWorkTemplateModel);
        }
        [SwaggerOperation(Summary = "cập nhật công việc mẫu {Authorize = Admin}")]
        [HttpPut("id")]
        public async Task<ApiResponse<bool>> UpdateWorkTemplateAsync(Guid id, UpdateWorkTemplateModel updateWorkTemplateModel)
        {
            return await _workTemplateService.UpdateWorkTemplateAsync(id, updateWorkTemplateModel);
        }
        [SwaggerOperation(Summary = "xóa công việc mẫu {Authorize = Admin}")]
        [HttpDelete("id")]
        public async Task<ApiResponse<bool>> DeleteWorkTemplateByIdAsync(Guid id)
        {
            return await _workTemplateService.DeleteWorkTemplateByIdAsync(id);
        }
    }
}

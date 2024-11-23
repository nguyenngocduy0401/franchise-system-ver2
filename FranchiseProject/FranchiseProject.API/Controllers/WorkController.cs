﻿using FranchiseProject.Application.Commons;
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
            AppRole.SystemConsultant + AppRole.SystemTechnician + ","
            + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "cập nhật trạng thái công việc {Authorize = Manager, SystenConsultant, SystemTechniciaan, SystemInstructor}")]
        [HttpPut("~/staff/api/v1/works/{id}/status")]
        public async Task<ApiResponse<bool>> UpdateWorkStatusSubmitByStaffAsync(Guid id, WorkStatusSubmitEnum workStatusSubmitEnum)
        {
            return await _workService.UpdateWorkStatusSubmitByStaffAsync(id, workStatusSubmitEnum);
        }
        [Authorize(Roles = AppRole.Manager + "," +
            AppRole.SystemConsultant + AppRole.SystemTechnician + ","
            + AppRole.SystemInstructor)]
        [SwaggerOperation(Summary = "nộp báo cáo công việc {Authorize = Manager, SystenConsultant, SystemTechniciaan, SystemInstructor}")]
        [HttpPut("~/staff/api/v1/works/{id}")]
        public async Task<ApiResponse<bool>> UpdateWorkByStaffAsync(Guid id,UpdateWorkByStaffModel updateWorkByStaffModel)
        {
            return await _workService.UpdateWorkByStaffAsync(id, updateWorkByStaffModel);
        }
    }
}

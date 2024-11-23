﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.AgenciesViewModels;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/agencies")]
    [ApiController]

    public class AgencyController : ControllerBase
    {
        private readonly IAgencyService _agencyService;
        private readonly IWorkService _workService;
        public AgencyController(IAgencyService agencyService, IWorkService workService)
        {
            _agencyService = agencyService;
            _workService = workService;
        }

        [Authorize(Roles = AppRole.Manager)]
        [SwaggerOperation(Summary = "Lấy công việc bằng agencyId{Authorize = Manager}")]
        [HttpGet("{id}/works")]
        public async Task<ApiResponse<IEnumerable<WorkViewModel>>> GetAllWorkByAgencyId(Guid id)
        {
            return await _workService.GetAllWorkByAgencyId(id);
        }

        [SwaggerOperation(Summary = "Đăng kí đối tác  ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> RegisterAgencyAsync(CreateAgencyViewModel create) => await _agencyService.CreateAgencyAsync(create);
        [SwaggerOperation(Summary = "cập nhật thông tin đối tác {Authorize = Manager,Admin} ")]
        [HttpPut("{id}")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]

        public async Task<ApiResponse<bool>> UpdateAgencyAsync(UpdateAgencyViewModel update, string id) => await _agencyService.UpdateAgencyAsync(update, id);
        [SwaggerOperation(Summary = "truy xuất thông tin đối tác  bằng Id {Authorize = Manager,Admin} ")]
        [HttpGet("{id}")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<AgencyViewModel>> GetAgencyById(string id) => await _agencyService.GetAgencyById(id);
        [SwaggerOperation(Summary = "truy xuất thông tin Agency {Authorize = Manager,Admin} ")]
        [HttpGet("")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<Pagination<AgencyViewModel>>> FilterAgencyAsync([FromQuery] FilterAgencyViewModel filter) => await _agencyService.FilterAgencyAsync(filter);
        [SwaggerOperation(Summary = "cập nhật trạng thái đối tác {Authorize = Manager,Admin} ")]
        [HttpPut("{id}/status")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<bool>> UpdateAgencyStatusAsync(string id, AgencyStatusEnum newStatus) => await _agencyService.UpdateAgencyStatusAsync(id, newStatus);

		[SwaggerOperation(Summary = "Lấy tất cả địa chỉ các chi nhánh đang hoạt động")]
		[HttpGet("active/addresses")]
		public async Task<ApiResponse<IEnumerable<AgencyAddressViewModel>>> GetActiveAgencyAdresses() => await _agencyService.GetActiveAgencyAdresses();
        [SwaggerOperation(Summary = "Lấy tất cả các chi nhánh đang hoạt động")]
        [HttpGet("active/agencies")]
        public async Task<ApiResponse<IEnumerable<AgencyNameViewModel>>> GetAllAgencyAsync() => await _agencyService.GetAllAgencyAsync();

    }
}

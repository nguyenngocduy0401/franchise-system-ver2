﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/agency")]
    [ApiController]
    public class AgencyController :ControllerBase
    {
        private readonly IAgencyService _agencyService;
        public AgencyController(IAgencyService agencyService )
        {
            _agencyService = agencyService; }
        [SwaggerOperation(Summary = "Đăng kí đối tác {Authorize = Manager} ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> RegisterAgencyAsync(CreateAgencyViewModel create)=> await _agencyService.CreateAgencyAsync(create);
        [SwaggerOperation(Summary = "cập nhật thông tin đối tác {Authorize = Manager} ")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateAgencyAsync(CreateAgencyViewModel update, string id)=> await _agencyService.UpdateAgencyAsync(update, id);
        [SwaggerOperation(Summary = "truy xuất thông tin đối tác  bằng Id {Authorize = Manager} ")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<AgencyViewModel>> GetAgencyById(string id) => await _agencyService.GetAgencyById(id);
        [SwaggerOperation(Summary = "truy xuất thông tin Agency {Authorize = Manager} ")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<AgencyViewModel>>> FilterAgencyAsync([FromQuery]FilterAgencyViewModel filter)=> await _agencyService.FilterAgencyAsync(filter);
        [SwaggerOperation(Summary = "cập nhật trạng thái đối tác {Authorize = Manager} ")]
        [HttpPut("")]
        public async Task<ApiResponse<bool>> UpdateAgencyStatusAsync(string id, AgencyStatusEnum newStatus)=> await _agencyService.UpdateAgencyStatusAsync(id, newStatus);
    }
}

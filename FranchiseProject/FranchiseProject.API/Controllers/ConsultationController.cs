using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/consultations")]
    public class ConsultationController
    {
        private readonly IConsultationService _franchiseRegistrationRequestService;
        public ConsultationController( IConsultationService franchiseRegistrationRequestService)
        {
            _franchiseRegistrationRequestService = franchiseRegistrationRequestService;
        }
        [SwaggerOperation(Summary = "khách đăng kí nhượng quyền ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> RegisterConsultationAsync([FromBody] RegisterConsultation regis)=> await _franchiseRegistrationRequestService.RegisterConsultationAsync(regis);
        [SwaggerOperation(Summary = "nhân viên tu vấn chuyển trạng thái 'đã tư vấn'")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string id) => await _franchiseRegistrationRequestService.UpdateConsultationStatusAsync(id);
        [SwaggerOperation(Summary = "lấy danh sách đăng kí tư vấn theo trạng thái")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel) => await _franchiseRegistrationRequestService.FilterConsultationAsync(filterModel);
    }
}
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/consultations")]
    public class ConsultationController
    {
        private readonly IRegisterFormSevice _consultationService;
        public ConsultationController( IRegisterFormSevice consultationService)
        {
            _consultationService = consultationService;
        }
     //   [Authorize(Roles = AppRole.SystemConsultant + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "khách đăng kí nhượng quyền ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> RegisterConsultationAsync([FromBody] RegisterConsultationViewModel regis)=> await _consultationService.RegisterConsultationAsync(regis);
        [SwaggerOperation(Summary = "nhân viên tu vấn chuyển trạng thái 'đã tư vấn'{Authorize = SystemConsultant}")]
        [HttpPut("{id}")]
        [Authorize(Roles = AppRole.SystemConsultant)]
        public async Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string id) => await _consultationService.UpdateConsultationStatusAsync(id);
        [SwaggerOperation(Summary = "lấy danh sách đăng kí tư vấn theo trạng thái{Authorize = SystemConsultant}")]
        [HttpGet("")]
        [Authorize(Roles = AppRole.SystemConsultant)]
        public async Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel) => await _consultationService.FilterConsultationAsync(filterModel);
    }
}
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/franchiseRegistrationRequests")]
    public class FranchiseRegistrationRequestController
    {
        private readonly IFranchiseRegistrationRequestService _franchiseRegistrationRequestService;
        public FranchiseRegistrationRequestController( IFranchiseRegistrationRequestService franchiseRegistrationRequestService)
        {
            _franchiseRegistrationRequestService = franchiseRegistrationRequestService;
        }
        [SwaggerOperation(Summary = "khách đăng kí nhượng quyền ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> RegisterFranchiseAsync([FromBody] RegisFranchiseViewModel regis)=> await _franchiseRegistrationRequestService.RegisterFranchiseAsync(regis);
        [SwaggerOperation(Summary = "nhân viên tu vấn chuyển trạng thái 'đã tư vấn'")]
        [HttpPut("id")]
        public async Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string id) => await _franchiseRegistrationRequestService.UpdateConsultationStatusAsync(id);
        [SwaggerOperation(Summary = "lấy danh sách đăng kí tư vấn theo trạng thái")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<FranchiseRegistrationRequestsViewModel>>> FilterFranchiseRegistrationRequestAsync(FilterFranchiseRegistrationRequestsViewModel filterModel) => await _franchiseRegistrationRequestService.FilterFranchiseRegistrationRequestAsync(filterModel);
    }
}
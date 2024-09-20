using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
<<<<<<< HEAD:FranchiseProject/FranchiseProject.API/Controllers/FranchiseRegistrationRequestController.cs
    [Route("api/v1/franchiseRegistrationRequests")]
    [ApiController]
    public class FranchiseRegistrationRequestController : ControllerBase
=======
    [Route("api/v1/consultations")]
    public class ConsultationController
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a:FranchiseProject/FranchiseProject.API/Controllers/ConsultationController.cs
    {
        private readonly IConsultationService _franchiseRegistrationRequestService;
        public ConsultationController( IConsultationService franchiseRegistrationRequestService)
        {
            _franchiseRegistrationRequestService = franchiseRegistrationRequestService;
        }
        [SwaggerOperation(Summary = "khách đăng kí nhượng quyền ")]
        [HttpPost("")]
<<<<<<< HEAD:FranchiseProject/FranchiseProject.API/Controllers/FranchiseRegistrationRequestController.cs
        public async Task<ApiResponse<bool>> RegisterFranchiseAsync([FromBody] RegisterFranchiseViewModel regis)=> await _franchiseRegistrationRequestService.RegisterFranchiseAsync(regis);
=======
        public async Task<ApiResponse<bool>> RegisterConsultationAsync([FromBody] RegisterConsultation regis)=> await _franchiseRegistrationRequestService.RegisterConsultationAsync(regis);
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a:FranchiseProject/FranchiseProject.API/Controllers/ConsultationController.cs
        [SwaggerOperation(Summary = "nhân viên tu vấn chuyển trạng thái 'đã tư vấn'")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string id) => await _franchiseRegistrationRequestService.UpdateConsultationStatusAsync(id);
        [SwaggerOperation(Summary = "lấy danh sách đăng kí tư vấn theo trạng thái")]
        [HttpGet("")]
<<<<<<< HEAD:FranchiseProject/FranchiseProject.API/Controllers/FranchiseRegistrationRequestController.cs
        public async Task<ApiResponse<Pagination<FranchiseRegistrationRequestsViewModel>>> FilterFranchiseRegistrationRequestAsync(FilterFranchiseRegistrationRequestsViewModel filterModel) => await _franchiseRegistrationRequestService.FilterFranchiseRegistrationRequestAsync(filterModel);

=======
        public async Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel) => await _franchiseRegistrationRequestService.FilterConsultationAsync(filterModel);
>>>>>>> fe3ee3b3bca4e0caa1da32b242e99a2c4327a23a:FranchiseProject/FranchiseProject.API/Controllers/ConsultationController.cs
    }
}
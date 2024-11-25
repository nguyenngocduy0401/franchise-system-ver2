using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/contracts")]
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [SwaggerOperation(Summary = "upload hợp đồng {Authorize = Manager,Admin}")]
        [HttpPut("")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<bool>> UploadContractAsync(CreateContractViewModel create)
        {
            return await _contractService.UploadContractAsync(create);
        }

        [SwaggerOperation(Summary = "Cập nhật hợp đồng {Authorize = Manager,Admin}")]
        [HttpPut("{id}")]

        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<bool>> UpdateContractAsync([FromBody] UpdateContractViewModel update, string id)
        {
            return await _contractService.UpdateContractAsync(update, id);
        }

        [SwaggerOperation(Summary = "Truy xuất hợp đồng bằng Id {Authorize = Manager,Admin}")]
        [HttpGet("{id}")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<ContractViewModel>> GetContractById(string id)
        {
            return await _contractService.GetContractByIdAsync(id);
        }

        [SwaggerOperation(Summary = "Truy xuất hợp đồng (StarTime,EndTime) {Authorize = Manager,Admin}")]
        [HttpGet("")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public Task<ApiResponse<Pagination<ContractViewModel>>> FilterContractAsync([FromQuery] FilterContractViewModel filter)
        {
            return _contractService.FilterContractViewModelAsync(filter);
        }
        [SwaggerOperation(Summary = "Truy xuất thông tin đối tác cho hợp đồng  {Authorize = Manager,Admin}")]
        [HttpGet("agency/{id}")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public Task<ApiResponse<AgencyInfoViewModel>> GetAgencyInfoAsync(Guid id)
        {
            return _contractService.GetAgencyInfoAsync(id);
        }
        [SwaggerOperation(Summary = "Tải xuống hợp đồng dưới dạng file .doc {Authorize = Manager,Admin}")]
        [HttpGet("download/agency/{id}")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<IActionResult> DownloadContractAsync(Guid id)
        {
            try
            {
                var fileResult = await _contractService.DownloadContractAsPdfAsync(id);
                return fileResult;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

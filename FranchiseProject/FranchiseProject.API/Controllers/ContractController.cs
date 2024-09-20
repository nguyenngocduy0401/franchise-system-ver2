using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ContractViewModel;
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

        [SwaggerOperation(Summary = "Tạo hợp đồng {Authorize = Manager,Admin}")]
        [HttpPost("")]
        [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        public async Task<ApiResponse<bool>> CreateContractAsync([FromBody] CreateContractViewModel create)
        {
            return await _contractService.CreateContractAsync(create);
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
    }
}

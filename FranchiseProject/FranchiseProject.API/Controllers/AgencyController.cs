using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyViewModel;
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
        [SwaggerOperation(Summary = "tao agency {Authorize = Manager} ")]
        [HttpPost("")]
        public async Task<ApiResponse<bool>> CreateAgencyAsync(CreateAgencyViewModel create)=> await _agencyService.CreateAgencyAsync(create);
        [SwaggerOperation(Summary = "cap nhat agency {Authorize = Manager} ")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateAgencyAsync(CreateAgencyViewModel update, string id)=> await _agencyService.UpdateAgencyAsync(update, id);
        [SwaggerOperation(Summary = "truy xuat agency by Id {Authorize = Manager} ")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<AgencyViewModel>> GetAgencyById(string id) => await _agencyService.GetAgencyById(id);
        [SwaggerOperation(Summary = "truy xuat agency {Authorize = Manager} ")]
        [HttpGet("")]
        Task<ApiResponse<Pagination<AgencyViewModel>>> FilterAgencyAsync(FilterAgencyViewModel filter)=> _agencyService.FilterAgencyAsync(filter);
    }
}

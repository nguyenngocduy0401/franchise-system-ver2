using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.TermViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/terms")]
    [ApiController]
    public class TermController : ControllerBase
    {
        private readonly ITermService _termService;

        public TermController(ITermService termService)
        {
            _termService = termService;
        }
      //  [Authorize(Roles = AppRole.Admin + "," + AppRole.Manager)]
        [SwaggerOperation(Summary = "Tạo học kỳ {Authorize = FrachiseManager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateTermAsync(CreateTermViewModel createTermViewModel)
        {
            return await _termService.CreateTermAsync(createTermViewModel);
        }

       
        [SwaggerOperation(Summary = "xóa học kỳ by Id {Authorize = FrachiseManager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteTermAsync(Guid id)
        {
            return await _termService.DeleteTermByIdAsync(id.ToString());
        }

        
        [SwaggerOperation(Summary = "Cập nhật học kỳ {Authorize = FrachiseManager}}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateTermAsync(Guid id, CreateTermViewModel updateTermViewModel)
        {
            return await _termService.UpdateTermAsync(updateTermViewModel, id.ToString());
        }

        [SwaggerOperation(Summary = "truy xuất học kỳ by Id ")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<TermViewModel>> GetTermByIdAsync(Guid id)
        {
            return await _termService.GetTermByIdAsync(id.ToString());
        }

        [SwaggerOperation(Summary = "truy xuất học kỳ")]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<TermViewModel>>> FilterTermAsync([FromQuery] FilterTermViewModel filterTermViewModel)
        {
            return await _termService.FilterTermAsync(filterTermViewModel);
        }
        [SwaggerOperation(Summary = "truy xuất tất cả  học kỳ")]
        [HttpGet()]
        public async Task<ApiResponse<Pagination<TermViewModel>>> GetAllTermAsync(int pageSize, int pageIndex)
        {
            return await _termService.GetAllTermAsync(pageSize, pageIndex);
        }
    }
}
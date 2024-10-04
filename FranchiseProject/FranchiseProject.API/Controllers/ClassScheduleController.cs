using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{

        [Route("api/v1/class-schedules")]
        [ApiController]
        public class ClassScheduleController : ControllerBase
        {
            private readonly IClassScheduleService _classScheduleService;

            public ClassScheduleController(IClassScheduleService classScheduleService)
            {
                _classScheduleService = classScheduleService;
            }

       // [Authorize(Roles = AppRole.FranchiseManager)]
        [SwaggerOperation(Summary = "xóa lịch học bằng id (FranchiseManager)")]
            [HttpDelete("{id}")]
            public async Task<ApiResponse<bool>> DeleteClassScheduleAsync(string id)
            {
                return await _classScheduleService.DeleteClassScheduleByIdAsync(id);
            }

            [SwaggerOperation(Summary = "tạo mới lịch học (FranchiseManager)")]
            [HttpPost]
            public async Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel)
            {
                return await _classScheduleService.CreateClassScheduleAsync(createClassScheduleViewModel);
            }

            [SwaggerOperation(Summary = "tạo mới lịch học theo khoảng thời gian (FranchiseManager)")]
            [HttpPost("date-range")]
            public async Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync([FromBody] CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel)
            {
                return await _classScheduleService.CreateClassScheduleDateRangeAsync(createClassScheduleDateRangeViewModel);
            }

            [SwaggerOperation(Summary = "cập nhật lịch học (FranchiseManager)")]
            [HttpPut("{id}")]
            public async Task<ApiResponse<bool>> UpdateClassScheduleAsync(string id, CreateClassScheduleViewModel updateClassScheduleViewModel)
            {
                return await _classScheduleService.UpdateClassScheduleAsync(updateClassScheduleViewModel, id);
            }

            [SwaggerOperation(Summary = "tìm lịch học bằng id")]
            [HttpGet("{id}")]
            public async Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id)
            {
                return await _classScheduleService.GetClassScheduleByIdAsync(id);
            }

            [SwaggerOperation(Summary = "tìm kiếm lịch học")]
            [HttpGet]
            public async Task<ApiResponse<Pagination<ClassScheduleViewModel>>> FilterClassScheduleAsync([FromQuery] FilterClassScheduleViewModel filterClassScheduleViewModel)
            {
                return await _classScheduleService.FilterClassScheduleAsync(filterClassScheduleViewModel);
            }
        }
    }
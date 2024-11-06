using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AttendanceViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
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
        [SwaggerOperation(Summary = "xóa lịch học bằng id  {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpDelete("{id}")]
            public async Task<ApiResponse<bool>> DeleteClassScheduleAsync(string id)
            {
                return await _classScheduleService.DeleteClassScheduleByIdAsync(id);
            }

            [SwaggerOperation(Summary = "tạo mới lịch học  {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost]
            public async Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel)
            {
                return await _classScheduleService.CreateClassScheduleAsync(createClassScheduleViewModel);
            }

            [SwaggerOperation(Summary = "tạo mới lịch học theo khoảng thời gian  {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost("date-range")]
            public async Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync([FromBody] CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel)
            {
                return await _classScheduleService.CreateClassScheduleDateRangeAsync(createClassScheduleDateRangeViewModel);
            }

            [SwaggerOperation(Summary = "cập nhật lịch học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPut("{id}")]
            public async Task<ApiResponse<bool>> UpdateClassScheduleAsync(string id, CreateClassScheduleViewModel updateClassScheduleViewModel)
            {
                return await _classScheduleService.UpdateClassScheduleAsync(updateClassScheduleViewModel, id);
            }
/*
            [SwaggerOperation(Summary = "tìm lịch học bằng id {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("{id}")]
            public async Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id)
            {
                return await _classScheduleService.GetClassScheduleByIdAsync(id);
            }*/

            [SwaggerOperation(Summary = "tìm kiếm lịch học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet]
            public async Task<ApiResponse<List<ClassScheduleViewModel>>> FilterClassScheduleAsync([FromQuery]FilterClassScheduleViewModel filterClassScheduleViewModel)
            {
                    return await _classScheduleService.FilterClassScheduleAsync(filterClassScheduleViewModel);
                }
            [SwaggerOperation(Summary = "xóa tất cả lịch học của 1 lớp {Authorize = AgencyManager ,AgencyStaff}")]
            [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
            [HttpDelete("~/api/v1/classes/{id}/class-schedules")]
            public async Task<ApiResponse<bool>> DeleteClassSheduleAllByClassIdAsync(string id)
            {
                return await _classScheduleService.DeleteClassSheduleAllByClassIdAsync(id);
            }
        [SwaggerOperation(Summary = "lấy thông tin chi tiết của 1 lịch học   {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("{id}")]
        public Task<ApiResponse<ClassScheduleDetailViewModel>> GetClassScheduleDetailAsync(Guid id)=>_classScheduleService.GetClassScheduleDetailAsync(id);
    }
    }
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/attendances")]
    [ApiController]
    public class AttendanceController :ControllerBase
    {
        private IAttendanceService _attendanceService;
        public AttendanceController(IAttendanceService attendanceService) { _attendanceService = attendanceService; }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager + "," + AppRole.Instructor)]
        [SwaggerOperation(Summary = "điểm danh học sinh  {Authorize = AgencyStaff, AgencyManager,Instructor}")]
        [HttpPost]
        public async Task<ApiResponse<bool>> MarkAttendanceByClassScheduleAsync(Guid classScheduleId, List<string> studentIds)
        {
            return await _attendanceService.MarkAttendanceByClassScheduleAsync(classScheduleId,studentIds);
        }
    }
}

using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/register-course")]
    [ApiController]

    public class RegisterCourseController:ControllerBase
    {
        private readonly IRegisterCourseService _registerCourseService;
        public RegisterCourseController(IRegisterCourseService registerCourseService)
        {
            _registerCourseService = registerCourseService;
        }

        [SwaggerOperation(Summary = "học sinh đăng kí khóa học ")]
        [HttpPost]
        public async Task<ApiResponse<bool>> RegisterCourseAsync(RegisterCourseViewModel model)=>await _registerCourseService.RegisterCourseAsync(model);
        [SwaggerOperation(Summary = "Cập nhật trạng thái học sinh từ Notconsul thành Pending  {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPut("{id}")]

        public async  Task<ApiResponse<bool>> UpdateStatusStudentAsync( string id,string courseId,StudentCourseStatusEnum status) => await _registerCourseService.UpdateStatusStudentAsync(id,courseId,status);
        [SwaggerOperation(Summary = "lấy thông tin học sinh by Id   {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("{id}")]
        public async Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync(string id, string courseId) => await _registerCourseService.GetStudentRegisterByIdAsync(id,courseId);
        [SwaggerOperation(Summary = "Lấy thông tin đăng kí học sinh {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<StudentRegisterViewModel>>> FilterStudentAsync([FromQuery]FilterRegisterCourseViewModel filterStudentModel)=> await _registerCourseService.FilterStudentAsync(filterStudentModel);

        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [SwaggerOperation(Summary = "cập nhật thông tin đăng kí {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPut("{userId}/{courseId}")]
        public async Task<ApiResponse<bool>> UpdateRegisterCourseDateTimeAsync(string userId, string courseId,[FromBody] UpdateRegisterCourseViewModel update)=> await _registerCourseService.UpdateRegisterCourseDateTimeAsync(userId,courseId,update);
    }
}

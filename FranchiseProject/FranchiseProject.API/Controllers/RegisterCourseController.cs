using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Enums;
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
        [SwaggerOperation(Summary = "Cập nhật trạng thái học sinh  {Authorize = AgencyManager ,AgencyStaff}")]
        [HttpPut("{id}")]
        public async  Task<ApiResponse<bool>> UpdateStatusStudentAsync(StudentStatusEnum studentStatus, string studentId)=> await _registerCourseService.UpdateStatusStudentAsync(studentStatus, studentId);
        [SwaggerOperation(Summary = "lấy thông tin học sinh by Id   {Authorize = AgencyManager ,AgencyStaff}")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync(string id) => await _registerCourseService.GetStudentRegisterByIdAsync(id);
        [SwaggerOperation(Summary = "Lấy thông tin đăng kí học sinh {Authorize = AgencyManager,")]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<StudentViewModel>>> FilterStudentAsync([FromQuery]FilterRegisterCourseViewModel filterStudentModel)=> await _registerCourseService.FilterStudentAsync(filterStudentModel);    
    }
}

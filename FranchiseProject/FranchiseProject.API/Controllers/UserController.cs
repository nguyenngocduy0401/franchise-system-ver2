using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClassService _classService;
        private readonly IRegisterCourseService _registerCourseService;
        private readonly IAssignmentService _assignmentService;
        public UserController(IUserService userService, IClassService classService, IRegisterCourseService registerCourseService,IAssignmentService assignmentService)
        {
            _userService = userService;
            _classService = classService;
            _registerCourseService = registerCourseService;
            _assignmentService = assignmentService;
        }

        [SwaggerOperation(Summary = "lấy thông tin User bằng đăng nhập")]
        [HttpGet("mine")]
        public async Task<ApiResponse<UserViewModel>> GetInfoByLoginAsync() => await _userService.GetInfoByLoginAsync();
     
        [SwaggerOperation(Summary = "lấy thông tin User bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id) => await _userService.GetUserByIdAsync(id);

        /*[Authorize(Roles = AppRole.Admin)]*/
        [SwaggerOperation(Summary = "tìm kiếm người dùng {Authorize = Administrator}")]
        [HttpGet("~/admin/api/v1/users")]
        public async Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync([FromQuery]FilterUserByAdminModel filterUserByAdminModel)
            => await _userService.FilterUserByAdminAsync(filterUserByAdminModel);

        [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "cấm và bỏ lệnh cấm người dùng {Authorize = Administrator}")]
        [HttpPut("~/admin/api/v1/users/{id}/status")]
        public async Task<ApiResponse<bool>> BanAndUnbanUserByAdminAsync(string id)
            => await _userService.BanAndUnbanUserByAdminAsync(id);

        [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = Administrator}")]
        [HttpPost("~/admin/api/v1/users")]
        public async Task<ApiResponse<CreateUserByAdminModel>> CreateUserByAdminAsync(CreateUserByAdminModel createUserByAdminModel)
            => await _userService.CreateUserByAdminAsync(createUserByAdminModel);

        [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "cập nhật người dùng {Authorize = Administrator}")]
        [HttpPut("~/admin/api/v1/users")]
        public async Task<ApiResponse<bool>> UpdateUserByAdminAsync(string id, UpdateUserByAdminModel updateUserByAdminModel)
            => await _userService.UpdateUserByAdminAsync(id, updateUserByAdminModel);

        [Authorize()]
        [SwaggerOperation(Summary = "đổi mật khẩu người dùng")]
        [HttpPost("change-password")]
        public async Task<ApiResponse<bool>> ChangePasswordAsync(UpdatePasswordModel updatePasswordModel)
            => await _userService.ChangePasswordAsync(updatePasswordModel);
        [Authorize(Roles = AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = AgencyManager}")]
        [HttpPost("~/agency-manager/api/v1/users")]
        public async Task<ApiResponse<CreateUserViewModel>> CreateUserByAgencyAsync(CreateUserByAgencyModel createUserByAgencyModel)
            => await _userService.CreateUserByAgencyAsync(createUserByAgencyModel);
        [Authorize(Roles = AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = AgencyManager}")]
        [HttpPost("~/agency-manager/api/v1/users/files")]
        public async Task<ApiResponse<List<CreateUserByAgencyModel>>> CreateListUserByAgencyAsync(IFormFile file)
            => await _userService.CreateListUserByAgencyAsync(file);
        [Authorize(Roles = AppRole.Student)]
        [SwaggerOperation(Summary = "học sinh đăng kí khóa học  {Authorize = Student}")]
        [HttpPost("mine/courses")]
        public async Task<ApiResponse<bool>> StudentExistRegisterCourse(string courseId)
           => await _registerCourseService.StudentExistRegisterCourse(courseId);

         [SwaggerOperation(Summary = "người dùng lấy lịch học bằng by login")]
         [HttpGet("mine/class-schedules")]
         public async Task<ApiResponse<List<StudentScheduleViewModel>>> GetStudentSchedulesAsync(DateTime startTime, DateTime endTime) => await _classService.GetStudentSchedulesAsync(startTime,endTime);
        [SwaggerOperation(Summary = "giáo viên chấm điểm bài tập (assignement){Authorize = Instructor}")]
        [Authorize(Roles = AppRole.Instructor)]
        [HttpPost("~/instructor/api/v1/scores")]
        public async Task<ApiResponse<bool>> GradeStudentAssAsync(StudentAssScorseNumberViewModel model) => await _assignmentService.GradeStudentAssAsync(model);
        [SwaggerOperation(Summary = "học sinh nộp bài tập (assignement){Authorize = Student}")]
        [Authorize(Roles = AppRole.Student)]
        [HttpPost("mine/assignments")]
        public async Task<ApiResponse<bool>> SubmitAssignmentAsync(string assignmentId, string fileSubmitUrl)=> await _assignmentService.SubmitAssignmentAsync(assignmentId, fileSubmitUrl);
        [SwaggerOperation(Summary = "Lấy danh sách lớp học bằng login  (class){Authorize = Student,Instructor}")]
        [Authorize(Roles = AppRole.Student+","+AppRole.Instructor)]
        [HttpGet("mine/classes")]
        public async Task<ApiResponse<List<ClassByLoginViewModel>>> GetAllClassByLogin()=> await _classService.GetAllClassByLogin();
    }
}

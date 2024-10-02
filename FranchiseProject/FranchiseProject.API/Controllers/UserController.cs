using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [SwaggerOperation(Summary = "lấy thông tin User bằng đăng nhập")]
        [HttpGet("mine")]
        public async Task<ApiResponse<UserViewModel>> GetInfoByLoginAsync() => await _userService.GetInfoByLoginAsync();
     
        [SwaggerOperation(Summary = "lấy thông tin User bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<UserViewModel>> GetUserByIdAsync(string id) => await _userService.GetUserByIdAsync(id);

        [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "tìm kiếm người dùng {Authorize = Administrator}")]
        [HttpGet("~/admin/api/v1/users")]
        public async Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync([FromQuery]FilterUserByAdminModel filterUserByAdminModel)
            => await _userService.FilterUserByAdminAsync(filterUserByAdminModel);

        [Authorize(Roles = AppRole.Admin)]
        [SwaggerOperation(Summary = "xóa người dùng {Authorize = Administrator}")]
        [HttpDelete("~/admin/api/v1/users")]
        public async Task<ApiResponse<bool>> DeleteUserByAdminAsync(string id)
            => await _userService.DeleteUserByAdminAsync(id);

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
        /*[Authorize(Roles = AppRole.AgencyManager)]*/
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = AgencyManager}")]
        [HttpPost("~/agency-manager/api/v1/users")]
        public async Task<ApiResponse<CreateUserViewModel>> CreateUserByAgencyAsync(CreateUserByAgencyModel createUserByAgencyModel)
            => await _userService.CreateUserByAgencyAsync(createUserByAgencyModel);
        /*[Authorize(Roles = AppRole.AgencyManager)]*/
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = AgencyManager}")]
        [HttpPost("~/agency-manager/api/v1/users/files")]
        public async Task<ApiResponse<List<CreateUserByAgencyModel>>> CreateListUserByAgencyAsync(IFormFile file)
            => await _userService.CreateListUserByAgencyAsync(file);

    }
}

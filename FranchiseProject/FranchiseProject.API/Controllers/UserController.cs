using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
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
        
        [SwaggerOperation(Summary = "tìm kiếm người dùng {Authorize = Administrator}")]
        [HttpGet("~/admin/api/v1/users")]
        public async Task<ApiResponse<Pagination<UserViewModel>>> FilterUserByAdminAsync([FromQuery]FilterUserByAdminModel filterUserByAdminModel)
            => await _userService.FilterUserByAdminAsync(filterUserByAdminModel);
        [SwaggerOperation(Summary = "xóa người dùng {Authorize = Administrator}")]
        [HttpDelete("~/admin/api/v1/users")]
        public async Task<ApiResponse<bool>> DeleteUserByAdminAsync(string id)
            => await _userService.DeleteUserByAdminAsync(id);
        [SwaggerOperation(Summary = "tạo người dùng {Authorize = Administrator}")]
        [HttpPost("~/admin/api/v1/users")]
        public async Task<ApiResponse<bool>> CreateUserByAdminAsync(CreateUserByAdminModel createUserByAdminModel)
            => await _userService.CreateUserByAdminAsync(createUserByAdminModel);
        [SwaggerOperation(Summary = "cập nhật người dùng {Authorize = Administrator}")]
        [HttpPut("~/admin/api/v1/users")]
        public async Task<ApiResponse<bool>> UpdateUserByAdminAsync(string id, UpdateUserByAdminModel updateUserByAdminModel)
            => await _userService.UpdateUserByAdminAsync(id, updateUserByAdminModel);

    }
}

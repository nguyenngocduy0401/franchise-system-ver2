using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/assignments")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "tạo mới bài tập {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPost]
        public async Task<ApiResponse<bool>> CreateAssignmentAsync(CreateAssignmentViewModel assignment)
        {
            return await _assignmentService.CreateAssignmentAsync(assignment);
        }

        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "cập nhật bài tập {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateAssignmentAsync(string id, [FromBody] CreateAssignmentViewModel update)
        {
            return await _assignmentService.UpdateAssignmentAsync(update, id);
        }

        [SwaggerOperation(Summary = "tìm bài tập bằng ID")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<AssignmentViewModel>> GetAssignmentByIdAsync(string id)
        {
            return await _assignmentService.GetAssignmentByIdAsync(id);
        }

        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "xóa bài tập bằng ID {Authorize = AgencyStaff, AgencyManager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteAssignmentAsync(string id)
        {
            return await _assignmentService.DeleteSlotByIdAsync(id);
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "lấy danhh sách bài tập của một lớp  {Authorize = AgencyStaff, AgencyManager}")]
        [HttpGet("assignments/{id}/submissions")]
       public async Task<ApiResponse<Pagination<AssignmentViewModel>>> GetAssignmentByClassIdAsync(string id, int pageIndex, int pageSize)
        {
            return await _assignmentService.GetAssignmentByClassIdAsync(id, pageIndex, pageSize);
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "lấy danhh sách bài tập đã nộp   {Authorize = AgencyStaff, AgencyManager}")]
        [HttpGet("classes/{id}/assignments")]
        public async Task<ApiResponse<Pagination<AssignmentSubmitViewModel>>> GetAssignmentSubmissionAsync(string assignmentId, int pageIndex, int pageSize)
        {
            return await _assignmentService.GetAssignmentSubmissionAsync(assignmentId, pageIndex, pageSize);
        }
    }
}

using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/classes")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

       // [Authorize(Roles = AppRole.Admin)]
       [SwaggerOperation(Summary = "Tạo mới lớp học {Authorize = AgencyManager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel createClassModel)
        {
            return await _classService.CreateClassAsync(createClassModel);
        }
        [SwaggerOperation(Summary = "Cập nhật lớp học {Authorize = AgencyManager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateClassAsync(string id, UpdateClassViewModel model)
        {
            return await _classService.UpdateClassAsync(id,model);
        }
        [SwaggerOperation(Summary = "Tìm kiếm lớp học{Authorize = AgencyManager,")]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync([FromQuery] FilterClassViewModel filterClassModel)
        {
            return await _classService.FilterClassAsync(filterClassModel);
        }
        [SwaggerOperation(Summary = "Cập nhật Trạng thái lớp học  {Authorize = AgencyManager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateClassStatusAsync(ClassStatusEnum status, string id)
        {
            return await _classService.UpdateClassStatusAsync(status, id);
        }

        [SwaggerOperation(Summary = "Tìm kiếm lớp học{Authorize = AgencyManager,")]
        [HttpGet("")]
        public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetClassDetailAsync(string id)
        {
            return await _classService.GetClassDetailAsync(id);
        }


        [SwaggerOperation(Summary = "Xóa lớp học{Authorize = AgencyManager,")]
                [HttpDelete("{id}")]
                public async Task<ApiResponse<bool>> DeleteClassAsync(string id) => await _classService.DeleteClassAsync(id);
                // GET: api/v1/classes/{id}/students (Lấy danh sách học sinh trong lớp học)
                [SwaggerOperation(Summary = "Lấy danh sách học sinh trong lớp học{Authorize = AgencyManager,")]
                [HttpGet("{id}/students")]
                public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetStudentsInClassAsync(Guid id)
                {
                    return await _classService.GetListStudentInClassAsync(id.ToString());
                }
                [SwaggerOperation(Summary = "Lấy danh sách lớp học chưa có lịch học {Authorize = AgencyManager,")]
                [HttpGet("/class-schedules")]
                public async Task<ApiResponse<Pagination<ClassViewModel>>> GetClassesWithoutScheduleAsync(int pageIndex, int pageSize) => await _classService.GetClassesWithoutScheduleAsync(pageIndex, pageSize);*/
    }
}

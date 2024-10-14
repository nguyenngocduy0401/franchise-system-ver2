using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassViewModel;
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
        [SwaggerOperation(Summary = "Tạo mới lớp học {Authorize = Admin, Manager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel createClassModel)
        {
            return await _classService.CreateClassAsync(createClassModel);
        }

    
        [SwaggerOperation(Summary = "Cập nhật lớp học {Authorize = Admin, Manager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateClassAsync(Guid id, CreateClassViewModel updateClassModel)
        {
            return await _classService.UpdateClassAsync(updateClassModel, id.ToString());
        }

        [SwaggerOperation(Summary = "Tìm lớp học bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<ClassViewModel>> GetClassByIdAsync(Guid id)
        {
            return await _classService.GetClassByIdAsync(id.ToString());
        }

        [SwaggerOperation(Summary = "Tìm kiếm lớp học")]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync([FromQuery] FilterClassViewModel filterClassModel)
        {
            return await _classService.FilterClassAsync(filterClassModel);
        }


        // GET: api/v1/classes/{id}/students (Lấy danh sách học sinh trong lớp học)
        [SwaggerOperation(Summary = "Lấy danh sách học sinh trong lớp học")]
        [HttpGet("{id}/students")]
        public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetStudentsInClassAsync(Guid id)
        {
            return await _classService.GetListStudentInClassAsync(id.ToString());
        }
    }
}

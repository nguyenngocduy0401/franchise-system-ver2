﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        [SwaggerOperation(Summary = "Tạo mới lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateClassAsync([FromBody] CreateClassViewModel createClassModel)
        {
            return await _classService.CreateClassAsync(createClassModel);
        }

        [SwaggerOperation(Summary = "Cập nhật lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateClassAsync(string id, [FromBody] UpdateClassViewModel model)
        {
            return await _classService.UpdateClassAsync(id, model);
        }

        [SwaggerOperation(Summary = "Tìm kiếm lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("filter")]
        public async Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync([FromQuery] FilterClassViewModel filterClassModel)
        {
            return await _classService.FilterClassAsync(filterClassModel);
        }

        [SwaggerOperation(Summary = "Cập nhật trạng thái lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPatch("{id}/status")]
        public async Task<ApiResponse<bool>> UpdateClassStatusAsync( ClassStatusEnum status, string id)
        {
            return await _classService.UpdateClassStatusAsync(status, id);
        }

        [SwaggerOperation(Summary = "Lấy thông tin chi tiết lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpGet("{id}")]
        public async Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetClassDetailAsync(string id)
        {
            return await _classService.GetClassDetailAsync(id);
        }

        [SwaggerOperation(Summary = "Xóa lớp học {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteClassAsync(string id)
        {
            return await _classService.DeleteClassAsync(id);
        }

        [SwaggerOperation(Summary = "Xóa học sinh khỏi lớp {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpDelete("{classId}/users/{studentId}")]
        public async Task<ApiResponse<bool>> RemoveStudentAsync(string studentId, string classId)
        {
            return await _classService.RemoveStudentAsync(studentId, classId);
        }

        [SwaggerOperation(Summary = "Thêm học sinh vào lớp {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost("{classId}/users")]
        public async Task<ApiResponse<bool>> AddStudentAsync(string classId,[FromBody] AddStudentViewModel model)
        {
            return await _classService.AddStudentAsync(classId, model);
        }
        [SwaggerOperation(Summary = "lấy lịch học của lớp bằng ClassId {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost("class-schedules/{classId}")]
        public async Task<ApiResponse<List<ClassScheduleViewModel>>> GetClassSchedulesByClassIdAsync(string classId)
        {
            return await _classService.GetClassSchedulesByClassIdAsync(classId);
        }
        [SwaggerOperation(Summary = "lấy danh sách giáo viên  {Authorize = AgencyManager ,AgencyStaff}")]
        [Authorize(Roles = AppRole.AgencyManager + "," + AppRole.AgencyStaff)]
        [HttpPost("~/api/v1/agencies/instructors")]
        public async Task<ApiResponse<List<InstructorViewModel>>> GetInstructorsByAgencyAsync()
        {
            return await _classService.GetInstructorsByAgencyAsync();
        }
    }
}

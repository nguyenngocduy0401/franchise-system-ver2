﻿using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/slots")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _slotService;
        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "xóa slot bằng id {Authorize = AgencyStaff, AgencyManager}")]
        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteSlotAsync(Guid id)
        {
            return await _slotService.DeleteSlotByIdAsync(id);
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "tạo mới slot{Authorize = AgencyStaff, AgencyManager}")]
        [HttpPost()]
        public async Task<ApiResponse<bool>> CreateSlotAsync(CreateSlotModel createSlotModel)
        {
            return await _slotService.CreateSlotAsync(createSlotModel);
        }
        [Authorize(Roles = AppRole.AgencyStaff + "," + AppRole.AgencyManager)]
        [SwaggerOperation(Summary = "cập nhật slot {Authorize = AgencyStaff, AgencyManager}")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateSlotAsync(Guid id, CreateSlotModel updateSlotModel)
        {
            return await _slotService.UpdateSlotAsync(id, updateSlotModel);
        }
        [SwaggerOperation(Summary = "tìm slot bằng id")]
        [HttpGet("{id}")]
        public async Task<ApiResponse<SlotViewModel>> GetSlotByIdAsync(Guid id)
        {
            return await _slotService.GetSlotByIdAsync(id);
        }
        [SwaggerOperation(Summary = "tìm kiếm slot")]
        [HttpGet("~/manager/api/v1/slots")]
        public async Task<ApiResponse<Pagination<SlotViewModel>>> FilterSlotAsync([FromQuery]FilterSlotModel filterSlotModel)
        {
            return await _slotService.FilterSlotAsync(filterSlotModel);
        }
        [SwaggerOperation(Summary = "lấy tất cả slot")]
        [HttpGet()]
        public async Task<ApiResponse<List<SlotViewModel>>> GetAllSlotAsync()
        {
            return await _slotService.GetAllSlotAsync();
        }
        [SwaggerOperation(Summary = "lấy tất cả slot bằng AgencyId")]
        [HttpGet("agency/{id}")]
        public async Task<ApiResponse<List<SlotViewModel>>> GetAllSlotAsyncByAgencyId(Guid id)
        {
            return await _slotService.GetAllSlotAsyncByAgencyId(id);
        }
    }
}

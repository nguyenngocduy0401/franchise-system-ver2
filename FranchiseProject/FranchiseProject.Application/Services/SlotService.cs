using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateSlotModel> _slotValidator;
        private readonly IMapper _mapper;
        public SlotService(IUnitOfWork unitOfWork, IValidator<CreateSlotModel> slotValidator, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _slotValidator = slotValidator;
            _mapper = mapper;
        }
        public async Task<ApiResponse<Pagination<SlotViewModel>>> FilterSlotAsync(FilterSlotModel filterSlotModel)
        {
            var response = new ApiResponse<Pagination<SlotViewModel>>();
            try
            {
                Expression<Func<Slot, bool>> filter = s =>
                ((!filterSlotModel.StartTime.HasValue || filterSlotModel.StartTime <= s.StartTime) &&
                (!filterSlotModel.StartTime.HasValue || filterSlotModel.EndTime >= s.EndTime) &&
                (!filterSlotModel.IsDeleted.HasValue || s.IsDeleted == filterSlotModel.IsDeleted));
                var slots = await _unitOfWork.SlotRepository.GetFilterAsync(
                    filter: filter,
                    pageIndex: filterSlotModel.PageIndex,
                    pageSize: filterSlotModel.PageSize
                    );
                var slotViewModels = _mapper.Map<Pagination<SlotViewModel>>(slots);
                response.Data = slotViewModels;
                response.isSuccess = true;
                response.Message = "lấy danh sách slot thành công!";

            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteSlotByIdAsync(Guid slotId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
                if (slot == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy slot!";
                    return response;
                }
                switch (slot.IsDeleted) 
                {
                    case false:
                        _unitOfWork.SlotRepository.SoftRemove(slot);
                        response.Message = "Xoá slot học thành công!";
                        break;
                    case true:
                        _unitOfWork.SlotRepository.RestoreSoftRemove(slot);
                        response.Message = "Phục hồi slot học thành công!";
                        break;
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete fail!");
                response.Data = true;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<SlotViewModel>> GetSlotByIdAsync(Guid slotId)
        {
            var response = new ApiResponse<SlotViewModel>();
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
                if (slot == null) {
                    response.Data = null;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy slot!";
                    return response;
                }
                var slotViewModel = _mapper.Map<SlotViewModel>(slot);
                response.Data = slotViewModel;
                response.isSuccess = true;
                response.Message = "tìm slot học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateSlotAsync(Guid slotId,CreateSlotModel updateSlotModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _slotValidator.ValidateAsync(updateSlotModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
                slot.StartTime = updateSlotModel.StartTime;
                slot.EndTime =updateSlotModel.EndTime;
                _unitOfWork.SlotRepository.Update(slot);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Udpate fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật slot học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateSlotAsync(CreateSlotModel createSlotModel) 
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _slotValidator.ValidateAsync(createSlotModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var slot = _mapper.Map<Slot>(createSlotModel);
                await _unitOfWork.SlotRepository.AddAsync(slot);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo slot học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

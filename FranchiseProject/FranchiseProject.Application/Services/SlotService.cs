using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
                (!filterSlotModel.StartTime.HasValue || filterSlotModel.StartTime <= s.StartTime) &&
                (!filterSlotModel.StartTime.HasValue || filterSlotModel.EndTime >= s.EndTime);
                var slots = await _unitOfWork.SlotRepository.GetFilterAsync(
                    filter: filter,
                    pageIndex: filterSlotModel.PageIndex,
                    pageSize: filterSlotModel.PageSize
                    );
                var slotViewModels = _mapper.Map<Pagination<SlotViewModel>>(slots);
                if (slotViewModels.Items.IsNullOrEmpty()) return ResponseHandler.Success(slotViewModels, "Không tìm thấy slot phù hợp!");

                response = ResponseHandler.Success(slotViewModels, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<SlotViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteSlotByIdAsync(Guid slotId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetExistByIdAsync(slotId);
                if (slot == null) return ResponseHandler.Failure<bool>("Slot không tồn tại!");

                _unitOfWork.SlotRepository.SoftRemove(slot);
                  
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                
                response = ResponseHandler.Success(true, "Xoá slot học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<SlotViewModel>> GetSlotByIdAsync(Guid slotId)
        {
            var response = new ApiResponse<SlotViewModel>();
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
                if (slot == null) throw new Exception("Slot does not exist!");

                var slotViewModel = _mapper.Map<SlotViewModel>(slot);
                response = ResponseHandler.Success(slotViewModel, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<SlotViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateSlotAsync(Guid slotId,CreateSlotModel updateSlotModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _slotValidator.ValidateAsync(updateSlotModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var slot = await _unitOfWork.SlotRepository.GetExistByIdAsync(slotId);
                if(slot == null) return ResponseHandler.Failure<bool>("Slot không tồn tại!");
                slot = _mapper.Map(updateSlotModel, slot);
                _unitOfWork.SlotRepository.Update(slot);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "cập nhật slot học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateSlotAsync(CreateSlotModel createSlotModel) 
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _slotValidator.ValidateAsync(createSlotModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var slot = _mapper.Map<Slot>(createSlotModel);
                await _unitOfWork.SlotRepository.AddAsync(slot);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo slot học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<SlotViewModel>>> GetAllSlotAsync()
        {
            var response = new ApiResponse<List<SlotViewModel>>();
            try
            {
                var slot = await _unitOfWork.SlotRepository.GetAllAsync(query => query.OrderBy(p => p.Name));
                var slotViewModel = _mapper.Map<List<SlotViewModel>>(slot);
                response = ResponseHandler.Success(slotViewModel, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<SlotViewModel>>(ex.Message);
            }
            return response;
        }
    }
}

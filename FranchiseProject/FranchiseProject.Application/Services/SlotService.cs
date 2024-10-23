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
        private readonly UserManager<User> _userManager;
        private readonly IClaimsService _claimsService;
        public SlotService(IClaimsService claimsService,IUnitOfWork unitOfWork, IValidator<CreateSlotModel> slotValidator, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _slotValidator = slotValidator;
            _mapper = mapper;
            _userManager = userManager;
            _claimsService= claimsService;
        }
        public async Task<ApiResponse<Pagination<SlotViewModel>>> FilterSlotAsync(FilterSlotModel filterSlotModel)
        {
            var response = new ApiResponse<Pagination<SlotViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Failure<Pagination<SlotViewModel>>("User hoặc Agency không khả dụng!");
                }
                Expression<Func<Slot, bool>> filter = s =>
                (!filterSlotModel.StartTime.HasValue || filterSlotModel.StartTime <= s.StartTime) &&
                (!filterSlotModel.StartTime.HasValue || filterSlotModel.EndTime >= s.EndTime)&&
                 (s.AgencyId == userCurrent.AgencyId);
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
                if (slot == null) return ResponseHandler.Failure<bool>("Slot học không khả dụng!");

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
                    var userCurrentId = _claimsService.GetCurrentUserId;
                    var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                    if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                    {
                        return ResponseHandler.Failure<SlotViewModel>("User hoặc Agency không khả dụng!");
                    }
                    var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
                    if (slot == null) throw new Exception("Slot không tồn tại!");
                    if (slot.AgencyId != userCurrent.AgencyId)
                    {
                        return ResponseHandler.Failure<SlotViewModel>("Bạn không có quyền truy cập slot này vì nó không thuộc Agency của bạn!");
                    }
                    var slotViewModel = _mapper.Map<SlotViewModel>(slot);
                    response = ResponseHandler.Success(slotViewModel, "Lấy thông tin slot thành công!");
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
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Failure<bool>("User hoặc Agency không khả dụng!");
                }
                ValidationResult validationResult = await _slotValidator.ValidateAsync(updateSlotModel);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);
                }
                var slot = await _unitOfWork.SlotRepository.GetExistByIdAsync(slotId);
                if (slot == null) return ResponseHandler.Failure<bool>("Slot không tồn tại!");
                if (slot.AgencyId != userCurrent.AgencyId)
                {
                    return ResponseHandler.Failure<bool>("Bạn không có quyền cập nhật slot này vì nó không thuộc Agency của bạn!");
                }
                slot = _mapper.Map(updateSlotModel, slot);
                _unitOfWork.SlotRepository.Update(slot);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Cập nhật thất bại!");

                response = ResponseHandler.Success(true, "Cập nhật slot thành công!");
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
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent =await _userManager.FindByIdAsync(userCurrentId);
                
                ValidationResult validationResult = await _slotValidator.ValidateAsync(createSlotModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var slot = _mapper.Map<Slot>(createSlotModel);
                slot.AgencyId = userCurrent.AgencyId;
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
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                var slots = await _unitOfWork.SlotRepository.GetAllAsync(
              query => query.Where(s => s.AgencyId == userCurrent.AgencyId)
                            .OrderBy(s => s.Name));
  
                  var slotViewModel = _mapper.Map<List<SlotViewModel>>(slots);
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

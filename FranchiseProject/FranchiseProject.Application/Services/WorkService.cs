using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Application.ViewModels.UserViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class WorkService : IWorkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateWorkModel> _createWordValidator;
        private readonly IValidator<UpdateWorkModel> _updateWordValidator;
        public WorkService(IUnitOfWork unitOfWork, UserManager<User> userManager,
            IValidator<CreateWorkModel> createWorkValidator, IMapper mapper,
            IValidator<UpdateWorkModel> updateWordValidator, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _createWordValidator = createWorkValidator;
            _updateWordValidator = updateWordValidator;
            _claimsService = claimsService;
        }
        public async Task<ApiResponse<Pagination<WorkViewModel>>> FilterWorksByLogin(FilterWorkByLoginModel filterWorkByLoginModel)
        {
            var response = new ApiResponse<Pagination<WorkViewModel>>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var filter = (Expression<Func<Work, bool>>)(e =>
                    (string.IsNullOrEmpty(filterWorkByLoginModel.Search) || e.Title.Contains(filterWorkByLoginModel.Search)
                    || e.Description.Contains(filterWorkByLoginModel.Search)) &&
                    (filterWorkByLoginModel.Status.HasValue || e.Status == filterWorkByLoginModel.Status) &&
                    (filterWorkByLoginModel.Level.HasValue || e.Level == filterWorkByLoginModel.Level)
                );
                var worksPagination = await _unitOfWork.WorkRepository.FilterWorksByUserId(
                userId: userId,
                filter: filter,
                pageIndex: filterWorkByLoginModel.PageIndex,
                pageSize: filterWorkByLoginModel.PageSize
                );
                var workModel = _mapper.Map<Pagination<WorkViewModel>>(worksPagination);

                response = ResponseHandler.Success(workModel, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<WorkViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<WorkDetailViewModel>> GetWorkDetailByIdAsync(Guid id)
        {
            var response = new ApiResponse<WorkDetailViewModel>();
            try
            {
                var work = (await _unitOfWork.WorkRepository
                    .FindAsync(e => e.Id == id && e.IsDeleted != true, "Appointments")).FirstOrDefault();
                if (work == null) return ResponseHandler.Success(new WorkDetailViewModel(), "Nhiệm vụ không khả dụng!");

                var workModel = _mapper.Map<WorkDetailViewModel>(work);

                response = ResponseHandler.Success(workModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<WorkDetailViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteWorkByIdAsync(Guid workId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync(workId);
                if (work == null) return ResponseHandler.Success(false, "Nhiệm vụ không khả dụng!");

                _unitOfWork.WorkRepository.SoftRemove(work);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá nhiệm vụ thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<IEnumerable<WorkViewModel>>> GetAllWorkByAgencyId(Guid agencyId)
        {
            var response = new ApiResponse<IEnumerable<WorkViewModel>>();
            try
            {
                var work = _unitOfWork.WorkRepository.GetAllPreWorkByAgencyId(agencyId);
                var workModel = _mapper.Map<IEnumerable<WorkViewModel>>(work);

                response = ResponseHandler.Success(workModel, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<IEnumerable<WorkViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateWorkAsync(Guid workId, UpdateWorkModel updateWorkModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateWordValidator.ValidateAsync(updateWorkModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync(workId);
                if (work == null) return ResponseHandler.Success(false, "Nhiệm vụ không tồn tại!");

                work = _mapper.Map(updateWorkModel, work);
                _unitOfWork.WorkRepository.Update(work);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật nhiệm vụ thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateWorkAsync(CreateWorkModel createWorkModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createWordValidator.ValidateAsync(createWorkModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync((Guid)createWorkModel.AgencyId);
                if(agency == null) return ResponseHandler.Success(false, "Cơ sở nhượng quyền không khả dụng!");

                var work = _mapper.Map<Work>(createWorkModel);
                work.Status = WorkStatusEnum.None;
                work.Submit = WorkStatusSubmitEnum.None;
                await _unitOfWork.WorkRepository.AddAsync(work);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo nhiệm vụ thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateStatusWorkByIdAsync(Guid workId, WorkStatusEnum status)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync(workId);
                if (work == null) return ResponseHandler.Success(false, "Nhiệm vụ không khả dụng!");
                var checkWorkBefore = await _unitOfWork.WorkRepository
                    .FindAsync(e => e.IsDeleted != true &&
                               e.Type < work.Type && 
                               e.Status != WorkStatusEnum.Approved
                               );

                if(checkWorkBefore != null) return ResponseHandler.Success(false, "Phải hoàn thành nhiệm vụ ưu tiên trước!");

                switch (status) 
                {
                    case WorkStatusEnum.None:
                        throw new Exception("Cannot update this status!");
                    case WorkStatusEnum.Approved:
                        work.Status = status;
                        break;
                    case WorkStatusEnum.Rejected:
                        work.Status = status;
                        var agency = await _unitOfWork.AgencyRepository.GetExistByIdAsync(work.Id);
                        agency.Status = AgencyStatusEnum.Inactive;
                        _unitOfWork.AgencyRepository.Update(agency);
                        break;
                }
                
                _unitOfWork.WorkRepository.Update(work);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật nhiệm vụ thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

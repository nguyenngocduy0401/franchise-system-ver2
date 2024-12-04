using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class WorkTemplateService : IWorkTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateWorkTemplateModel> _createWorkTemplateValidator;
        private readonly IValidator<UpdateWorkTemplateModel> _updateWorkTemplateValidator;
        public WorkTemplateService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CreateWorkTemplateModel> createWorkTemplateValidator, IValidator<UpdateWorkTemplateModel> updateWorkTemplateValidator)
        {
            _createWorkTemplateValidator = createWorkTemplateValidator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _updateWorkTemplateValidator = updateWorkTemplateValidator;
        }
        public async Task<ApiResponse<WorkTemplateDetailViewModel>> GetWorkTemplateDetailByIdAsync(Guid id)
        {
            var response = new ApiResponse<WorkTemplateDetailViewModel>();
            try
            {
                var workTemplate = (await _unitOfWork.WorkTemplateRepository.FindAsync(e => e.Id == id, includeProperties: "Appointments")).FirstOrDefault();
                if (workTemplate == null) return ResponseHandler.Success(new WorkTemplateDetailViewModel(), "Nhiệm vụ không khả dụng!");
                var workModel = _mapper.Map<WorkTemplateDetailViewModel>(workTemplate);
                
                response = ResponseHandler.Success(workModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<WorkTemplateDetailViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<List<WorkTemplateViewModel>>> GetAllWorkTemplateAsync()
        {
            var response = new ApiResponse<List<WorkTemplateViewModel>>();
            try
            {
                var workTemplate = await _unitOfWork.WorkTemplateRepository.GetAllAsync();
                var workTemplateModel = _mapper.Map<List<WorkTemplateViewModel>>(workTemplate);
                
                response = ResponseHandler.Success(workTemplateModel, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<WorkTemplateViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateWorkTemplateAsync(Guid workTemplateId, UpdateWorkTemplateModel updateWorkTemplateModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateWorkTemplateValidator.ValidateAsync(updateWorkTemplateModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var workTemplate = await _unitOfWork.WorkTemplateRepository.GetExistByIdAsync(workTemplateId);
                workTemplate = _mapper.Map(updateWorkTemplateModel, workTemplate);

                _unitOfWork.WorkTemplateRepository.Update(workTemplate);

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
        public async Task<ApiResponse<bool>> CreateWorkTemplateAsync(CreateWorkTemplateModel createWorkTemplateModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createWorkTemplateValidator.ValidateAsync(createWorkTemplateModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var workTemplate = _mapper.Map<WorkTemplate>(createWorkTemplateModel);

                await _unitOfWork.WorkTemplateRepository.AddAsync(workTemplate);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo nhiệm vụ mẫu thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteWorkTemplateByIdAsync(Guid workTemplateId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var work = await _unitOfWork.WorkTemplateRepository.GetExistByIdAsync(workTemplateId);

                _unitOfWork.WorkTemplateRepository.SoftRemove(work);

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
    }
}

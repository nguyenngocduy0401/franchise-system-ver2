using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AppointmentTemplateViewModels;
using FranchiseProject.Application.ViewModels.WorkTemplateViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AppointmentTemplateService : IAppointmentTemplateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateAppointmentTemplateModel> _createAppointmentTemplateValidator;
        private readonly IValidator<UpdateAppointmentTemplateModel> _updateAppointmentTemplateValidator;
        public AppointmentTemplateService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CreateAppointmentTemplateModel> createAppointmentTemplateValidator, 
            IValidator<UpdateAppointmentTemplateModel> updateAppointmentTemplateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createAppointmentTemplateValidator = createAppointmentTemplateValidator;
            _updateAppointmentTemplateValidator = updateAppointmentTemplateValidator;
        }
        public async Task<ApiResponse<List<AppointmentTemViewModel>>> GetAllAppointmentTemplateByWorkIdAsync(Guid workId)
        {
            var response = new ApiResponse<List<AppointmentTemViewModel>>();
            try
            {
                var workTemplate = await _unitOfWork.AppointmentTemplateRepository
                    .FindAsync(e => e.WorkId == workId && e.IsDeleted != true);
                var workTemplateModel = _mapper.Map<List<AppointmentTemViewModel>>(workTemplate);

                response = ResponseHandler.Success(workTemplateModel, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<AppointmentTemViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateAppointmentTemplateAsync(Guid appointmentTemplateId, UpdateAppointmentTemplateModel updateAppointmentTemplateModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateAppointmentTemplateValidator.ValidateAsync(updateAppointmentTemplateModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var appointmentTemplate = await _unitOfWork.AppointmentTemplateRepository.GetExistByIdAsync(appointmentTemplateId);
                appointmentTemplate = _mapper.Map(updateAppointmentTemplateModel, appointmentTemplate);

                _unitOfWork.AppointmentTemplateRepository.Update(appointmentTemplate);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật lịch hẹn của nhiệm vụ mẫu thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateAppointmentTemplateAsync(CreateAppointmentTemplateModel createAppointmentTemplateModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAppointmentTemplateValidator.ValidateAsync(createAppointmentTemplateModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var appointmentTemplate = _mapper.Map<AppointmentTemplate>(createAppointmentTemplateModel);

                await _unitOfWork.AppointmentTemplateRepository.AddAsync(appointmentTemplate);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo lịch hẹn của nhiệm vụ mẫu thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteAppointmentTemplateByIdAsync(Guid appointmentTemplateId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var appointmentTemplate = await _unitOfWork.AppointmentTemplateRepository.GetExistByIdAsync(appointmentTemplateId);

                _unitOfWork.AppointmentTemplateRepository.SoftRemove(appointmentTemplate);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá lịch hẹn của nhiệm vụ thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

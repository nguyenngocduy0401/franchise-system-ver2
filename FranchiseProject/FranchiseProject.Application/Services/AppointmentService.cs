﻿using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateAppointmentModel> _createAppointmentValidator;
        private readonly IValidator<UpdateAppointmentModel> _updateAppointmentValidator;
        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper,
                IValidator<CreateAppointmentModel> createAppointmentValidator, IValidator<UpdateAppointmentModel> updateAppointmentValidator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createAppointmentValidator = createAppointmentValidator;
            _updateAppointmentValidator = updateAppointmentValidator;
        }
        public async Task<ApiResponse<AppointmentDetailViewModel>> GetAppointmentDetailByIdAsync(Guid id)
        {
            var response = new ApiResponse<AppointmentDetailViewModel>();
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository
                    .GetAppointmentAsyncById(id);
                if (appointment == null) return ResponseHandler
                        .Success(new AppointmentDetailViewModel(), "Cuộc hẹn không khả dụng!");

                var appointmentModel = _mapper.Map<AppointmentDetailViewModel>(appointment);

                response = ResponseHandler.Success(appointmentModel);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AppointmentDetailViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateAppointmentAsync(CreateAppointmentModel createAppointmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAppointmentValidator.ValidateAsync(createAppointmentModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync((Guid)createAppointmentModel.WorkId);
                if (work == null) return ResponseHandler.Success(false, "Cơ sở nhượng quyền không khả dụng!");

                if (createAppointmentModel.StartTime < work.StartDate || createAppointmentModel.EndTime > work.EndDate)
                    return ResponseHandler.Success(false, "Ngày trong cuộc hẹn phải nằm trong thời gian của nhiệm vụ!");

                var appointment = _mapper.Map<Appointment>(createAppointmentModel);
                appointment.Status = AppointmentStatusEnum.None;

                await _unitOfWork.AppointmentRepository.AddAsync(appointment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo cuộc hẹn thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteAppointmentAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetExistByIdAsync(id);
                var checkAppointmentAvailable = CheckAppointmentAvailable(appointment);
                if (checkAppointmentAvailable.Data == false) return checkAppointmentAvailable;


                _unitOfWork.AppointmentRepository.SoftRemove(appointment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo cuộc hẹn thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateAppointmentAsync(Guid id, UpdateAppointmentModel updateAppointmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetExistByIdAsync(id);
                var checkAppointmentAvailable = CheckAppointmentAvailable(appointment);
                if (checkAppointmentAvailable.Data == false) return checkAppointmentAvailable;

                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync((Guid)appointment.WorkId);
                if (updateAppointmentModel.StartTime < work.StartDate || updateAppointmentModel.EndTime > work.EndDate)
                    return ResponseHandler.Success(false, "Ngày trong cuộc hẹn phải nằm trong thời gian của nhiệm vụ!");

                appointment = _mapper.Map(updateAppointmentModel, appointment);
                _unitOfWork.AppointmentRepository.Update(appointment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật cuộc hẹn thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public ApiResponse<bool> CheckAppointmentAvailable(Appointment appointment)
        {
            var response = new ApiResponse<bool>();
            try
            {
                if (appointment == null) return ResponseHandler.Success(false, "Cuộc hẹn không khả dụng!");
                if (appointment.Status != AppointmentStatusEnum.None) return ResponseHandler.Success(false, "Cuộc hẹn không khả dụng!");
                response = ResponseHandler.Success(true);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

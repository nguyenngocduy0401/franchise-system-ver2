using AutoMapper;
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
        public async Task<ApiResponse<bool>> CreateAppointmentAsync(CreateAppointmentModel createAppointmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAppointmentValidator.ValidateAsync(createAppointmentModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var work = await _unitOfWork.WorkRepository.GetExistByIdAsync((Guid)createAppointmentModel.WorkId);
                if (work == null) return ResponseHandler.Success(false, "Cơ sở nhượng quyền không khả dụng!");

                if (work.StartDate <= createAppointmentModel.StartTime 
                    && createAppointmentModel.EndTime <= work.EndDate) 
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

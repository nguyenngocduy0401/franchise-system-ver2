using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSessionModel> _updateSessionValidator;
        private readonly IValidator<CreateSessionModel> _createSessionValidator;
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<UpdateSessionModel> updateSessionValidator, IValidator<CreateSessionModel> createSessionModel)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateSessionValidator = updateSessionValidator;
            _createSessionValidator = createSessionModel;
        }
        public async Task<ApiResponse<bool>> CreateSessionAsync(CreateSessionModel createSessionModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createSessionValidator.ValidateAsync(createSessionModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync((Guid)createSessionModel.CourseId);
                if (course == null) throw new Exception("Course does not exist!");

                var session = _mapper.Map<Session>(createSessionModel);
                await _unitOfWork.SessionRepository.AddAsync(session);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo buổi học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteSessionByIdAsync(Guid sessionId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy buổi học!";
                    return response;
                }
                switch (session.IsDeleted)
                {
                    case false:
                        _unitOfWork.SessionRepository.SoftRemove(session);
                        response.Message = "Xoá buổi học thành công!";
                        break;
                    case true:
                        _unitOfWork.SessionRepository.RestoreSoftRemove(session);
                        response.Message = "Phục hồi buổi học thành công!";
                        break;
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
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

        public async Task<ApiResponse<SessionViewModel>> GetSessionByIdAsync(Guid sessionId)
        {
            var response = new ApiResponse<SessionViewModel>();
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                var sessionModel = _mapper.Map<SessionViewModel>(session);
                response.Data = sessionModel;
                response.isSuccess = true;
                response.Message = "Successful!";
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateSessionAsync(Guid sessionId, UpdateSessionModel updateSessionModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateSessionValidator.ValidateAsync(updateSessionModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var session = await _unitOfWork.SessionRepository.GetExistByIdAsync(sessionId);
                session = _mapper.Map(updateSessionModel, session);
                _unitOfWork.SessionRepository.Update(session);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật buổi học thành công!";

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

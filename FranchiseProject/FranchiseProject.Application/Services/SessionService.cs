using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.SessionViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class SessionService : ISessionService
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSessionModel> _updateSessionValidator;
        private readonly IValidator<CreateSessionModel> _createSessionValidator;
        private readonly IValidator<List<CreateSessionArrangeModel>> _createSessionArrangeValidator; 
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService,
            IValidator<UpdateSessionModel> updateSessionValidator, IValidator<CreateSessionModel> createSessionModel,
            IValidator<List<CreateSessionArrangeModel>> createSessionArrangeValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _courseService = courseService;
            _updateSessionValidator = updateSessionValidator;
            _createSessionValidator = createSessionModel;
            _createSessionArrangeValidator = createSessionArrangeValidator;
        }
        public async Task<ApiResponse<bool>> CreateSessionAsync(CreateSessionModel createSessionModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createSessionValidator.ValidateAsync(createSessionModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(createSessionModel.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var session = _mapper.Map<Session>(createSessionModel);
                await _unitOfWork.SessionRepository.AddAsync(session);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo phiên học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteSessionByIdAsync(Guid sessionId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var session = await _unitOfWork.SessionRepository.GetExistByIdAsync(sessionId);
                if (session == null) return ResponseHandler.Success<bool>(false, "Phiên học của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(session.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.SessionRepository.SoftRemove(session);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá phiên học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<SessionViewModel>> GetSessionByIdAsync(Guid sessionId)
        {
            var response = new ApiResponse<SessionViewModel>();
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null) throw new Exception("Session does not exist!");
                var sessionModel = _mapper.Map<SessionViewModel>(session);
                response = ResponseHandler.Success(sessionModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<SessionViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateSessionAsync(Guid sessionId, UpdateSessionModel updateSessionModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateSessionValidator.ValidateAsync(updateSessionModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var session = await _unitOfWork.SessionRepository.GetExistByIdAsync(sessionId);
                if (session == null) return ResponseHandler.Success<bool>(false, "Phiên học của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(session.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                session = _mapper.Map(updateSessionModel, session);
                _unitOfWork.SessionRepository.Update(session);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật phiên học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateSessionArrangeAsync(Guid courseId, List<CreateSessionArrangeModel> createSessionArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createSessionArrangeValidator.ValidateAsync(createSessionArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var sessions = _mapper.Map<List<Session>>(createSessionArrangeModel);
                foreach (var session in sessions)
                {
                    session.CourseId = courseId;
                }
                var deleteSessions = (await _unitOfWork.SessionRepository.FindAsync(e => e.CourseId == courseId && e.IsDeleted != true)).ToList();
                if (!deleteSessions.IsNullOrEmpty()) _unitOfWork.SessionRepository.HardRemoveRange(deleteSessions);

                await _unitOfWork.SessionRepository.AddRangeAsync(sessions);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Phiên của khóa học được tạo thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

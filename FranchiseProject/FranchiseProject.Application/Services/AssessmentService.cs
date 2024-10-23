using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
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
    public class AssessmentService : IAssessmentService
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateAssessmentModel> _createAssessmentValidator;
        private readonly IValidator<UpdateAssessmentModel> _updateAssessmentValidator;
        private readonly IValidator<List<CreateAssessmentArrangeModel>> _createAssessmentArrangeValidator;
        public AssessmentService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService,
            IValidator<UpdateAssessmentModel> updateAssessmentValidator, IValidator<CreateAssessmentModel> createAssessmentValidator,
            IValidator<List<CreateAssessmentArrangeModel>> createAssessmentArrangeValidator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _courseService = courseService;
            _updateAssessmentValidator = updateAssessmentValidator;
            _createAssessmentValidator = createAssessmentValidator;
            _createAssessmentArrangeValidator = createAssessmentArrangeValidator;
        }
        public async Task<ApiResponse<bool>> CreateAssessmentAsync(CreateAssessmentModel createAssessmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAssessmentValidator.ValidateAsync(createAssessmentModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(createAssessmentModel.CourseId,
                    CourseStatusEnum.Draft
                    );
                if (!checkCourse.Data) return checkCourse;

                var assessment = _mapper.Map<Assessment>(createAssessmentModel);
                await _unitOfWork.AssessmentRepository.AddAsync(assessment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo đánh giá của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteAssessmentByIdAsync(Guid assessmentId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.GetExistByIdAsync(assessmentId);
                if (assessment == null) return ResponseHandler.Failure<bool>("Đánh giá của khóa học không khả dụng!");
                
                var checkCourse = await _courseService.CheckCourseAvailableAsync(assessment.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.AssessmentRepository.SoftRemove(assessment);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá đánh giá của khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public  async Task<ApiResponse<AssessmentViewModel>> GetAssessmentByIdAsync(Guid assessmentId)
        {
            var response = new ApiResponse<AssessmentViewModel>();
            try
            {
                var assessment = await _unitOfWork.AssessmentRepository.GetByIdAsync(assessmentId);
                if (assessment == null) throw new Exception("Assessment does not exist!");
                var assessmentModel = _mapper.Map<AssessmentViewModel>(assessment);
                response = ResponseHandler.Success(assessmentModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<AssessmentViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateAssessmentAsync(Guid assessmentId, UpdateAssessmentModel updateAssessmentModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateAssessmentValidator.ValidateAsync(updateAssessmentModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var assessment = await _unitOfWork.AssessmentRepository.GetExistByIdAsync(assessmentId);
                if (assessment == null) return ResponseHandler.Failure<bool>("Đánh giá của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(assessment.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                assessment = _mapper.Map(updateAssessmentModel, assessment);
                await _unitOfWork.AssessmentRepository.AddAsync(assessment);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật đánh giá của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateAssessmentArangeAsync(Guid courseId, List<CreateAssessmentArrangeModel> createAssessmentArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createAssessmentArrangeValidator.ValidateAsync(createAssessmentArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var assessments = _mapper.Map<List<Assessment>>(createAssessmentArrangeModel);
                foreach (var assessment in assessments)
                {
                    assessment.CourseId = courseId;
                }
                var deleteAssessments = (await _unitOfWork.AssessmentRepository.FindAsync(e => e.CourseId == courseId && e.IsDeleted != true)).ToList();
                if (!deleteAssessments.IsNullOrEmpty()) _unitOfWork.AssessmentRepository.SoftRemoveRange(deleteAssessments);

                await _unitOfWork.AssessmentRepository.AddRangeAsync(assessments);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo đánh giá khóa học thành công!");

            }

            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

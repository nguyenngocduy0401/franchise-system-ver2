using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.SyllabusViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class SyllabusService : ISyllabusService
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSyllabusModel> _updateSyllabusValidator;
        private readonly IValidator<CreateSyllabusModel> _createSyllabusValidator;
        public SyllabusService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateSyllabusModel> updateSyllabusValidator,
            IValidator<CreateSyllabusModel> createSyllabusValidator, ICourseService courseService)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateSyllabusValidator = updateSyllabusValidator;
            _createSyllabusValidator = createSyllabusValidator;
        }
        public async Task<ApiResponse<bool>> CreateSyllabusAsync(CreateSyllabusModel createSyllabusModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createSyllabusValidator.ValidateAsync(createSyllabusModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(createSyllabusModel.CourseId);
                var checkCourse = CheckCourseAvailableAsync(course);
                if (!checkCourse.Data) return checkCourse;

                var syllabus = _mapper.Map<Syllabus>(createSyllabusModel);
                await _unitOfWork.SyllabusRepository.AddAsync(syllabus);

                course.SyllabusId = syllabus.Id;
                _unitOfWork.CourseRepository.Update(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo thấy giáo trình của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteSyllabusByIdAsync(Guid syllabusId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var syllabus = await _unitOfWork.SyllabusRepository.GetExistByIdAsync(syllabusId);
                if (syllabus == null) return ResponseHandler.Failure<bool>("Giáo trình của khóa học không khả dụng!");

                var course = (await _unitOfWork.CourseRepository.FindAsync(e => e.SyllabusId == syllabusId && e.IsDeleted != true)).FirstOrDefault();
                var checkCourse = CheckCourseAvailableAsync(course);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.SyllabusRepository.SoftRemove(syllabus);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                response = ResponseHandler.Success(true, "Xoá giáo trình của khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<SyllabusViewModel>> GetSyllabusByIdAsync(Guid syllabusId)
        {
            var response = new ApiResponse<SyllabusViewModel>();
            try
            {
                var syllabus = (await _unitOfWork.SyllabusRepository
                    .FindAsync(e => e.Id == syllabusId, "Courses"))
                    .FirstOrDefault(); ;
                if(syllabus == null) throw new Exception("Syllabus does not exist!");
                var syllabusModel = _mapper.Map<SyllabusViewModel>(syllabus);
                response = ResponseHandler.Success(syllabusModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<SyllabusViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateSyllabusAsync(Guid syllabusId, UpdateSyllabusModel updateSyllabusModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateSyllabusValidator.ValidateAsync(updateSyllabusModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var syllabus = await _unitOfWork.SyllabusRepository.GetExistByIdAsync(syllabusId);
                if (syllabus == null) return ResponseHandler.Failure<bool>("Giáo trình của khóa học không khả dụng!");

                var course = (await _unitOfWork.CourseRepository.FindAsync(e => e.SyllabusId == syllabusId && e.IsDeleted != true)).FirstOrDefault();
                var checkCourse = CheckCourseAvailableAsync(course);

                if (!checkCourse.Data) return checkCourse;
                syllabus = _mapper.Map(updateSyllabusModel, syllabus);

                _unitOfWork.SyllabusRepository.Update(syllabus);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật giáo trình của khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        private ApiResponse<bool> CheckCourseAvailableAsync(Course course)
        {
            var response = new ApiResponse<bool>();
            var courseNoAvalable = "Khóa học không khả dụng!";
            var courseCanOnlyBeEditedInDraftState = "Chỉ có thể sửa đổi thông tin của khóa học ở trạng thái nháp!";
            try
            {

                if (course == null) return ResponseHandler.Success(false, courseNoAvalable);

                if (course.Status != CourseStatusEnum.Draft)
                {
                    response.Message = courseNoAvalable;
                    return ResponseHandler.Success(false, courseCanOnlyBeEditedInDraftState);
                }
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

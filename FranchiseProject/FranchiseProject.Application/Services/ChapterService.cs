using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICourseService _courseService;
        private readonly IValidator<UpdateChapterModel> _updateChapterValidator;
        private readonly IValidator<CreateChapterModel> _createChapterValidator;
        public ChapterService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService
            , IValidator<UpdateChapterModel> updateChapterValidator, IValidator<CreateChapterModel> createChapterValidator)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateChapterValidator = updateChapterValidator;
            _createChapterValidator = createChapterValidator;
        }
        public async Task<ApiResponse<bool>> CreateChapterAsync(CreateChapterModel createChapterModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createChapterValidator.ValidateAsync(createChapterModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(createChapterModel.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.isSuccess) return checkCourse;

                var chapter = _mapper.Map<Chapter>(createChapterModel);
                await _unitOfWork.ChapterRepository.AddAsync(chapter);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo chương học thành công!";

            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteChapterByIdAsync(Guid chapterId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(chapterId);
                if (chapter == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy chương học!";
                    return response;
                }
                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.isSuccess) return checkCourse; 

                _unitOfWork.ChapterRepository.SoftRemove(chapter);
                response.Message = "Xoá tài nguyên học thành công!";

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

        public async Task<ApiResponse<ChapterViewModel>> GetChapterByIdAsync(Guid chapterId)
        {
            var response = new ApiResponse<ChapterViewModel>();
            try
            {
                var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(chapterId);
                if (chapter == null) throw new Exception("Chapter does not exist!");
                var chapterlModel = _mapper.Map<ChapterViewModel>(chapter);
                response.Data = chapterlModel;
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

        public async Task<ApiResponse<bool>> UpdateChapterAsync(Guid chapterId, UpdateChapterModel updateChapterModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateChapterValidator.ValidateAsync(updateChapterModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync(chapterId);
                if (chapter == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy chương học!";
                    return response;
                }

                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.isSuccess) return checkCourse;

                chapter = _mapper.Map(updateChapterModel, chapter);

                _unitOfWork.ChapterRepository.Update(chapter);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "cập nhật chương học thành công!";

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

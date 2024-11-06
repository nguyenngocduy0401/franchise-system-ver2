using AutoMapper;
using ClosedXML.Excel;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ChapterMaterialViewModels;
using FranchiseProject.Application.ViewModels.ChapterViewModels;
using FranchiseProject.Application.ViewModels.QuestionOptionViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class ChapterMaterialService : IChapterMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICourseService _courseService;
        private readonly IValidator<UpdateChapterMaterialModel> _updateChapterMaterialValidator;
        private readonly IValidator<CreateChapterMaterialModel> _createChapterMaterialValidator;
        public ChapterMaterialService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService,
            IValidator<UpdateChapterMaterialModel> updateChapterMaterialValidator, IValidator<CreateChapterMaterialModel> createChapterMaterialValidator)
        {
            _courseService = courseService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateChapterMaterialValidator = updateChapterMaterialValidator;
            _createChapterMaterialValidator = createChapterMaterialValidator;
            
        }
       
        public async Task<ApiResponse<bool>> CreateChapterMaterialAsync(CreateChapterMaterialModel createChapterMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createChapterMaterialValidator.ValidateAsync(createChapterMaterialModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync((Guid)createChapterMaterialModel.ChapterId);
                if (chapter == null) return ResponseHandler.Success<bool>(false, "Chương học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var chapterMaterial = _mapper.Map<ChapterMaterial>(createChapterMaterialModel);
                await _unitOfWork.ChapterMaterialRepository.AddAsync(chapterMaterial);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo tài nguyên của chương học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteChapterMaterialByIdAsync(Guid chapterMaterialId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var chapterMaterial = await _unitOfWork.ChapterMaterialRepository.GetExistByIdAsync(chapterMaterialId);
                if (chapterMaterial == null) throw new Exception("ChapterMaterial does not exist!");

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync((Guid)chapterMaterial.ChapterId);
                if (chapter == null) return ResponseHandler.Success<bool>(false, "Chương học không khả dụng!");


                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.ChapterMaterialRepository.HardRemove(chapterMaterial);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá tài nguyên của chương học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateChapterMaterialAsync(Guid chapterMaterialId, UpdateChapterMaterialModel updateChapterMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateChapterMaterialValidator.ValidateAsync(updateChapterMaterialModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var chapterMaterial = await _unitOfWork.ChapterMaterialRepository.GetExistByIdAsync(chapterMaterialId);
                if (chapterMaterial == null) throw new Exception("ChapterMaterial does not exist!");

                var chapter = await _unitOfWork.ChapterRepository.GetExistByIdAsync((Guid)chapterMaterial.ChapterId);
                if (chapter == null) return ResponseHandler.Success<bool>(false, "Chương học không khả dụng!");


                var checkCourse = await _courseService.CheckCourseAvailableAsync(chapter.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                chapterMaterial = _mapper.Map(updateChapterMaterialModel, chapterMaterial);

                _unitOfWork.ChapterMaterialRepository.Update(chapterMaterial);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "cập nhật tài nguyên của chương học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

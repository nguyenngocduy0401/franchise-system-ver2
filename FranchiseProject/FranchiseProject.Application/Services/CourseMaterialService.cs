using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssessmentViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
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
    public class CourseMaterialService : ICourseMaterialService
    {
        private readonly ICourseService _courseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCourseMaterialModel> _createMaterialValidator;
        private readonly IValidator<UpdateCourseMaterialModel> _updateMaterialValidator;
        private readonly IValidator<List<CreateCourseMaterialArrangeModel>> _createMaterialArrangeValidator;
        public CourseMaterialService(IUnitOfWork unitOfWork, IMapper mapper, ICourseService courseService,
        IValidator<CreateCourseMaterialModel> createMaterialModel, IValidator<UpdateCourseMaterialModel> updateMaterialModel,
        IValidator<List<CreateCourseMaterialArrangeModel>> createMaterialArrangeValidator)
        {
            _courseService = courseService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createMaterialValidator = createMaterialModel;
            _updateMaterialValidator = updateMaterialModel;
            _createMaterialArrangeValidator = createMaterialArrangeValidator;
        }
        public async Task<ApiResponse<bool>> CreateCourseMaterialAsync(CreateCourseMaterialModel createMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createMaterialValidator.ValidateAsync(createMaterialModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(createMaterialModel.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var material = _mapper.Map<CourseMaterial>(createMaterialModel);
                await _unitOfWork.CourseMaterialRepository.AddAsync(material);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo tài nguyên học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteCourseMaterialByIdAsync(Guid materialId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var material = await _unitOfWork.CourseMaterialRepository.GetExistByIdAsync(materialId);
                if (material == null) return ResponseHandler.Failure<bool>("Tài nguyên của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(material.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.CourseMaterialRepository.SoftRemove(material);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá tài nguyên học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<CourseMaterialViewModel>> GetCourseMaterialByIdAsync(Guid materialId)
        {
            var response = new ApiResponse<CourseMaterialViewModel>();
            try 
            {
                var material = await _unitOfWork.CourseMaterialRepository.GetByIdAsync(materialId);
                if (material == null) throw new Exception("Material does not exist!");
                var matertialModel = _mapper.Map<CourseMaterialViewModel>(material);
                response = ResponseHandler.Success(matertialModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<CourseMaterialViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseMaterialAsync(Guid materialId, UpdateCourseMaterialModel updateMaterialModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateMaterialValidator.ValidateAsync(updateMaterialModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var material = await _unitOfWork.CourseMaterialRepository.GetExistByIdAsync(materialId);
                if (material == null) return ResponseHandler.Failure<bool>("Tài nguyên của khóa học không khả dụng!");

                var checkCourse = await _courseService.CheckCourseAvailableAsync(material.CourseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                material = _mapper.Map(updateMaterialModel, material);
                _unitOfWork.CourseMaterialRepository.Update(material);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật tài nguyên học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateMaterialArrangeAsync(Guid courseId, List<CreateCourseMaterialArrangeModel> createMaterialArrangeModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createMaterialArrangeValidator.ValidateAsync(createMaterialArrangeModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourse = await _courseService.CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                var materials = _mapper.Map<List<CourseMaterial>>(createMaterialArrangeModel);
                foreach (var material in materials) 
                {
                    material.CourseId = courseId;
                }
                var deleteMaterials = (await _unitOfWork.CourseMaterialRepository.FindAsync(e => e.CourseId == courseId && e.IsDeleted != true)).ToList();
                if(!deleteMaterials.IsNullOrEmpty()) _unitOfWork.CourseMaterialRepository.SoftRemoveRange(deleteMaterials);

                await _unitOfWork.CourseMaterialRepository.AddRangeAsync(materials);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(true, "Tạo tài nguyên học thành công!");

            }
            
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

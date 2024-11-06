using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateCourseCategoryModel> _createCourseCategoryValidator;
        private readonly IValidator<UpdateCourseCategoryModel> _updateCourseCategoryValidator;
        public CourseCategoryService(IUnitOfWork unitOfWork, IMapper mapper, 
            IValidator<UpdateCourseCategoryModel> updateCourseCategoryValidator, IValidator<CreateCourseCategoryModel> createCourseCategoryValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _updateCourseCategoryValidator = updateCourseCategoryValidator;
            _createCourseCategoryValidator = createCourseCategoryValidator;
        }
        public async Task<ApiResponse<List<CourseCategoryViewModel>>> GetAllCourseCategoryAsync()
        {
            var response = new ApiResponse<List<CourseCategoryViewModel>>();
            try
            {

                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetAllAsync();

                var courseCategoryModel = _mapper.Map<List<CourseCategoryViewModel>>(courseCategory);
                response.Data = courseCategoryModel;
                response.isSuccess = true;
                response.Message = "Successful!";
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<CourseCategoryViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> CreateCourseCategoryAsync(CreateCourseCategoryModel createCourseCategoryModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createCourseCategoryValidator.ValidateAsync(createCourseCategoryModel);
                if (!validationResult.IsValid) if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);
                var createCourse = _mapper.Map<CourseCategory>(createCourseCategoryModel);
                await _unitOfWork.CourseCategoryRepository.AddAsync(createCourse);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo loại của khóa học thành công!";
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteCourseCategoryByIdAsync(Guid courseCategoryId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetExistByIdAsync(courseCategoryId);
                if (courseCategory == null) return ResponseHandler.Success<bool>(false, "Loại của khóa học không khả dụng!");
                _unitOfWork.CourseCategoryRepository.SoftRemove(courseCategory);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                
                response = ResponseHandler.Success(true , "Xoá loại của khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<CourseCategoryViewModel>>> FilterCourseCategoryAsync(FilterCourseCategoryModel filterCourseCategoryModel)
        {
            var response = new ApiResponse<Pagination<CourseCategoryViewModel>>();
            try 
            {
                Expression<Func<CourseCategory, bool>> filter = e =>
                (string.IsNullOrEmpty(filterCourseCategoryModel.Search) || e.Name.Contains(filterCourseCategoryModel.Search) ||
                e.Description.Contains(filterCourseCategoryModel.Search));

                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetFilterAsync
                    (filter : filter,
                     pageIndex: filterCourseCategoryModel.PageIndex,
                     pageSize: filterCourseCategoryModel.PageSize
                    );
                var courseCategoryModel = _mapper.Map<Pagination<CourseCategoryViewModel>>(courseCategory);
                if (courseCategoryModel.Items.IsNullOrEmpty()) return ResponseHandler.Success(courseCategoryModel, "Không tìm thấy thấy loại của khóa học phù hợp!");
                response = ResponseHandler.Success(courseCategoryModel);
            }
            catch(Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<CourseCategoryViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<CourseCategoryViewModel>> GetCourseCategoryByIdAsync(Guid courseCategoryId)
        {
            var response = new ApiResponse<CourseCategoryViewModel>();
            try
            {
                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetByIdAsync(courseCategoryId);
                if (courseCategory == null) throw new Exception("Category does not exist!");
                var courseCategoryModel = _mapper.Map<CourseCategoryViewModel>(courseCategory);
                response = ResponseHandler.Success(courseCategoryModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<CourseCategoryViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseCategoryAsync(Guid courseCategoryId, UpdateCourseCategoryModel updateCourseCategoryModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetExistByIdAsync(courseCategoryId);
                if (courseCategory == null) return ResponseHandler.Success<bool>(false, "Loại của khóa học không khả dụng!");

                ValidationResult validationResult = await _updateCourseCategoryValidator.ValidateAsync(updateCourseCategoryModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                courseCategory = _mapper.Map(updateCourseCategoryModel, courseCategory);
                _unitOfWork.CourseCategoryRepository.Update(courseCategory);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");

                response = ResponseHandler.Success(true, "Cập nhật loại của khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

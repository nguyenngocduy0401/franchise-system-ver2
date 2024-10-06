using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseCategoryViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
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

        public async Task<ApiResponse<bool>> CreateCourseCategoryAsync(CreateCourseCategoryModel createCourseCategoryModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createCourseCategoryValidator.ValidateAsync(createCourseCategoryModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var createCourse = _mapper.Map<CourseCategory>(createCourseCategoryModel);
                await _unitOfWork.CourseCategoryRepository.AddAsync(createCourse);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Tạo loại của khóa học thành công!";
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public  async Task<ApiResponse<bool>> DeleteCourseCategoryByIdAsync(Guid courseCategoryId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetByIdAsync(courseCategoryId);
                if (courseCategory == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy loại của khóa học!";
                    return response;
                }
                switch (courseCategory.IsDeleted)
                {
                    case false:
                        _unitOfWork.CourseCategoryRepository.SoftRemove(courseCategory);
                        response.Message = "Xoá loại của khóa học thành công!";
                        break;
                    case true:
                        _unitOfWork.CourseCategoryRepository.RestoreSoftRemove(courseCategory);
                        response.Message = "Phục hồi loại của khóa học học thành công!";
                        break;
                }
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete fail!");
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

        public async Task<ApiResponse<Pagination<CourseCategoryViewModel>>> FilterCourseCategoryAsync(FilterCourseCategoryModel filterCourseCategoryModel)
        {
            var response = new ApiResponse<Pagination<CourseCategoryViewModel>>();
            try 
            {
                Expression<Func<CourseCategory, bool>> filter = e =>
                ((string.IsNullOrEmpty(filterCourseCategoryModel.Search) || e.Name.Contains(filterCourseCategoryModel.Search) ||
                e.Description.Contains(filterCourseCategoryModel.Search)) &&
                (!filterCourseCategoryModel.IsDeleted.HasValue || e.IsDeleted == filterCourseCategoryModel.IsDeleted));

                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetFilterAsync
                    (filter : filter,
                     pageIndex: filterCourseCategoryModel.PageIndex,
                     pageSize: filterCourseCategoryModel.PageSize
                    );
                var courseCategoryModel = _mapper.Map<Pagination<CourseCategoryViewModel>>(courseCategory);
                response.Data = courseCategoryModel;
                response.isSuccess = false;
                response.Message = "Successful!";
            }
            catch(Exception ex)
            {
                response.Data = null;
                response.isSuccess = false;
                response.Message = ex.Message;
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
                response.Data = courseCategoryModel;
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

        public async Task<ApiResponse<bool>> UpdateCourseCategoryAsync(Guid courseCategoryId, UpdateCourseCategoryModel updateCourseCategoryModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var courseCategory = await _unitOfWork.CourseCategoryRepository.GetExistByIdAsync(courseCategoryId);
                if (courseCategory == null) throw new Exception("Category does not exist!");
                ValidationResult validationResult = await _updateCourseCategoryValidator.ValidateAsync(updateCourseCategoryModel);
                if (!validationResult.IsValid)
                {
                    response.Data = false;
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                courseCategory = _mapper.Map(updateCourseCategoryModel, courseCategory);
                _unitOfWork.CourseCategoryRepository.Update(courseCategory);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create fail!");
                response.Data = true;
                response.isSuccess = true;
                response.Message = "Cập nhật loại của khóa học thành công!";
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

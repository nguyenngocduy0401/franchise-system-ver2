using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using FluentValidation.Results;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IValidator<CreateCourseModel> _createCourseValidator;
        private readonly IValidator<UpdateCourseModel> _updateCourseValidator;
        public CourseService(IUnitOfWork unitOfWork, IMapper mapper, 
            IValidator<UpdateCourseModel> updateCourseValidator, IValidator<CreateCourseModel> createCourseValidator,
            IClaimsService claimsService, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _createCourseValidator = createCourseValidator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _updateCourseValidator = updateCourseValidator;
            _claimsService = claimsService;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public Task<ApiResponse<bool>> CreateCourseVersionAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse<bool>> CreateCourseAsync(CreateCourseModel createCourseModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _createCourseValidator.ValidateAsync(createCourseModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var checkCourseExist =  await _unitOfWork.CourseRepository.FindAsync
                    (e => e.Code == createCourseModel.Code && e.IsDeleted != true
                    && e.Status != CourseStatusEnum.Closed);
                if (!checkCourseExist.IsNullOrEmpty()) 
                    return ResponseHandler.Success
                        (false,"Khóa học đã tồn tại không thể tạo mới. Chỉ có thể cập nhật lên phiên bản mới!" );

                var course = _mapper.Map<Course>(createCourseModel);
                await _unitOfWork.CourseRepository.AddAsync(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo chương học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Failure<bool>("Khóa học không khả dụng!");

                var checkCourse = await CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                _unitOfWork.CourseRepository.SoftRemove(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                response = ResponseHandler.Success(true, "Xoá tài khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<List<CourseViewModel>>> GetAllCourseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<CourseDetailViewModel>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetCourseDetailAsync(courseId);
                if (course == null) throw new Exception("Course does not exist!");
                var courseModel = _mapper.Map<CourseDetailViewModel>(course);
                response = ResponseHandler.Success(courseModel);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<CourseDetailViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseAsync(Guid courseId, UpdateCourseModel updateCourseModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                ValidationResult validationResult = await _updateCourseValidator.ValidateAsync(updateCourseModel);
                if (!validationResult.IsValid) return ValidatorHandler.HandleValidation<bool>(validationResult);

                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Failure<bool>("Khóa học không khả dụng!");

                var checkCourse = await CheckCourseAvailableAsync(courseId, CourseStatusEnum.Draft);
                if (!checkCourse.Data) return checkCourse;

                course = _mapper.Map(updateCourseModel, course);
                _unitOfWork.CourseRepository.Update(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật Khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<CourseViewModel>>> FilterCourseAsync(FilterCourseModel filterCourseViewModel)
        {
            var response = new ApiResponse<Pagination<CourseViewModel>>();
            try
            {
                Expression<Func<Course, bool>> filter = s =>
                (!filterCourseViewModel.MaxPrice.HasValue || filterCourseViewModel.MaxPrice >= s.Price) &&
                (!filterCourseViewModel.MinPrice.HasValue || filterCourseViewModel.MinPrice <= s.Price) &&
                (string.IsNullOrEmpty(filterCourseViewModel.Search) ||
                 s.Name.Contains(filterCourseViewModel.Search) ||
                 s.Description.Contains(filterCourseViewModel.Search) ||
                 s.Code.Contains(filterCourseViewModel.Search)) &&
                (!filterCourseViewModel.Status.HasValue || s.Status == filterCourseViewModel.Status) &&
                (!filterCourseViewModel.CourseCategoryId.HasValue || s.CourseCategoryId == filterCourseViewModel.CourseCategoryId);

                Func<IQueryable<Course>, IOrderedQueryable<Course>>? orderBy = null;
                if (filterCourseViewModel.SortBy.HasValue && filterCourseViewModel.SortDirection.HasValue)
                {
                    switch (filterCourseViewModel.SortBy) 
                    { 
                    case SortCourseStatusEnum.Name:
                        orderBy = filterCourseViewModel.SortDirection == SortDirectionEnum.Descending ?
                            query => query.OrderByDescending(p => p.Name) :
                            query => query.OrderBy(p => p.Name);
                        break;
                    case SortCourseStatusEnum.Price:
                        orderBy = filterCourseViewModel.SortDirection == SortDirectionEnum.Descending ?
                            query => query.OrderByDescending(p => p.Price) :
                            query => query.OrderBy(p => p.Price);
                        break;

                    }
                }
                var courses = await _unitOfWork.CourseRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties:"CourseCategory",
                    pageIndex: filterCourseViewModel.PageIndex,
                    pageSize: filterCourseViewModel.PageSize
                    );
                var courseViewModels = _mapper.Map<Pagination<CourseViewModel>>(courses);
                if (courseViewModels.Items.IsNullOrEmpty()) return ResponseHandler.Success(courseViewModels, "Không tìm thấy khóa học phù hợp!");

                response = ResponseHandler.Success(courseViewModels, "Successful!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<CourseViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> CheckCourseAvailableAsync(Guid? courseId, CourseStatusEnum status)
        {
            var response = new ApiResponse<bool>();
            var courseNoAvalable = "Khóa học không khả dụng!";
            var courseCanOnlyBeEditedInDraftState = "Chỉ có thể sửa đổi thông tin của khóa học ở trạng thái nháp!";
            try
            {
                if (courseId == null) throw new ArgumentNullException(nameof(courseId));
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync((Guid)courseId);
                if (course == null) return ResponseHandler.Success(false, courseNoAvalable);

                if (course.Status != status)
                {
                    response.Message = courseNoAvalable;
                    switch (status)
                    {
                        case CourseStatusEnum.Draft: response.Message = courseCanOnlyBeEditedInDraftState; break;
                        case CourseStatusEnum.AvailableForFranchise: response.Message = courseNoAvalable; break;
                    }
                    return ResponseHandler.Success(false, response.Message);
                }
                response = ResponseHandler.Success(true);
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateCourseStatusAsync(Guid courseId, CourseStatusEnum courseStatusEnum)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new Exception("Authenticate failed!");
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (role == null) throw new Exception("User role not found!");
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(courseId);
                if (course == null) return ResponseHandler.Failure<bool>("Khóa học không khả dụng!");
                switch (course.Status) 
                {
                    case CourseStatusEnum.Draft:
                        if ((courseStatusEnum != CourseStatusEnum.PendingApproval)) 
                            throw new Exception("Can only update to PendingApproval status");
                        if ((role != RolesEnum.SystemInstructor.ToString()) && (role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.PendingApproval:
                        if ((courseStatusEnum != CourseStatusEnum.AvailableForFranchise) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to AvailableForFranchise status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.AvailableForFranchise:
                        if ((courseStatusEnum != CourseStatusEnum.TemporarilySuspended) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to TemporarilySuspended status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.TemporarilySuspended:
                        if ((courseStatusEnum != CourseStatusEnum.AvailableForFranchise) && (courseStatusEnum != CourseStatusEnum.Closed))
                            throw new Exception("Can only update to AvailableForFranchise status or Closed status");
                        if ((role != RolesEnum.Manager.ToString()))
                            throw new Exception("Authorize Failed!");
                        break;
                    case CourseStatusEnum.Closed:
                            throw new Exception("Cannot update in this state");
                }
                course.Status = courseStatusEnum;
                _unitOfWork.CourseRepository.Update(course);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Update failed!");
                response = ResponseHandler.Success(true, "Cập nhật trạng thái khóa học thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
    }
}

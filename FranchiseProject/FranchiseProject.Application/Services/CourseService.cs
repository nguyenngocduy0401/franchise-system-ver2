using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
                if (course == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = courseNoAvalable;
                    return response;
                }
                if(course.Status != status)
                {
                    switch (status) 
                    {
                        case CourseStatusEnum.Draft: response.Message = courseCanOnlyBeEditedInDraftState; break;
                        case CourseStatusEnum.AvailableForFranchise: response.Message = courseNoAvalable; break;
                    }
                    response.Data = false;
                    response.isSuccess = true;
                    return response;
                }
                response.Data = true;
                response.isSuccess = true;
                response.Message = null;
                return response;
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public Task<ApiResponse<bool>> CreateCourseAsync(CreateSlotModel createCourseModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<bool>> DeleteCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "Không tìm thấy khóa học!";
                    return response;
                }

                _unitOfWork.CourseRepository.SoftRemove(course);
                response.Message = "Xoá tài khóa học thành công!";

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");
                response.Data = true;
                response.isSuccess = true;
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }

        public Task<ApiResponse<List<CourseViewModel>>> GetAllCourseAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<CourseDetailViewModel>();
            try
            {
                var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
                var courseModel = _mapper.Map<CourseDetailViewModel>(course);
                response.Data = courseModel;
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

        public Task<ApiResponse<bool>> UpdateCourseAsync(Guid materialId, CreateSlotModel updateCourseModel)
        {
            throw new NotImplementedException();
        }
    }
}

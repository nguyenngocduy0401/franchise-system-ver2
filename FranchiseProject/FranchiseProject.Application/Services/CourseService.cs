using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.MaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
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
                switch (course.IsDeleted)
                {
                    case false:
                        _unitOfWork.CourseRepository.SoftRemove(course);
                        response.Message = "Xoá tài khóa học thành công!";
                        break;
                    case true:
                        _unitOfWork.CourseRepository.RestoreSoftRemove(course);
                        response.Message = "Phục hồi khóa học thành công!";
                        break;
                }
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

        public Task<ApiResponse<List<MaterialViewModel>>> GetAllCourseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<MaterialViewModel>> GetCourseByIdAsync(Guid courseId)
        {
            var response = new ApiResponse<MaterialViewModel>();
            try 
            {
                var material = await _unitOfWork.MaterialRepository.GetByIdAsync(materialId);
                var matertialModel = _mapper.Map<MaterialViewModel>(material);
                response.Data = matertialModel;
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

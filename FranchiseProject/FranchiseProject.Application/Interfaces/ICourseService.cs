using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.CourseViewModels;
using FranchiseProject.Application.ViewModels.CourseMaterialViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FranchiseProject.Application.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse<bool>> UpdateCourseStatusAsync(Guid courseId, CourseStatusEnum courseStatusEnum);
        Task<ApiResponse<CourseDetailViewModel>> CreateCourseVersionAsync(Guid courseId);
        Task<ApiResponse<IEnumerable<CourseViewModel>>> GetAllCoursesAvailableAsync();
        Task<ApiResponse<Pagination<CourseViewModel>>> FilterCourseAsync(FilterCourseModel filterCourseViewModel);
        Task<ApiResponse<bool>> DeleteCourseByIdAsync(Guid courseId);
        Task<ApiResponse<CourseDetailViewModel>> GetCourseByIdAsync(Guid courseId);
        Task<ApiResponse<bool>> UpdateCourseAsync(Guid courseId, UpdateCourseModel updateCourseModel);
        Task<ApiResponse<bool>> CreateCourseAsync(CreateCourseModel createCourseModel);
        Task<ApiResponse<bool>> CheckCourseAvailableAsync(Guid? courseId, CourseStatusEnum courseStatus);
        Task<ApiResponse<bool>> CreateCourseByFileAsync(CourseFilesModel courseFilesModel);
        Task<ApiResponse<CourseStudentViewModel>> GetCourseByLoginAsync(Guid courseId);
    }
}

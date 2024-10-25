using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IRegisterCourseService
    {
    Task<ApiResponse<bool>> RegisterCourseAsync(RegisterCourseViewModel model);
    Task<ApiResponse<bool>> UpdateStatusStudentAsync(string studentId);
    Task<ApiResponse<StudentRegisterViewModel>> GetStudentRegisterByIdAsync (string id);
    Task<ApiResponse<Pagination<StudentRegisterViewModel>>> FilterStudentAsync(FilterRegisterCourseViewModel filterStudentModel);
        Task<ApiResponse<bool>> UpdateRegisterCourseDateTimeAsync(string userId, string courseId, string newDateTime);
    }
}

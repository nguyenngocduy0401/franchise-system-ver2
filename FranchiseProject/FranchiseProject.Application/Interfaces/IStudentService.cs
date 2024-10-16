using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModel;
using FranchiseProject.Application.ViewModels.StudentViewModels;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IStudentService
    {
        Task<ApiResponse<bool>>CreateStudentAsync(CreateStudentViewModel createStudentViewModel, string agencyId);
        Task<ApiResponse<bool>> UpdateStudentAsync(CreateStudentViewModel updateStudentViewModel, string studentId);
        Task<ApiResponse<bool>> DeleteStudentAsync(string studentId);
        Task<ApiResponse<StudentViewModel>> GetStudentByIdAsync(string studentId);
        Task<ApiResponse<Pagination<StudentViewModel>>> FilterStudentAsync(FilterStudentViewModel filter);
        Task<ApiResponse<bool>> UpdateStatusStudentAsync(StudentStatusEnum status, string studentId);
        Task<ApiResponse<int>> CountStudenInCourseAsync();
        Task<ApiResponse<bool>> StudentEnrollClassAsync(string classId);
        Task<ApiResponse<Pagination<ClassForStudentViewModel>>> GetClassForStudentAsync(string studentId);
    }
}

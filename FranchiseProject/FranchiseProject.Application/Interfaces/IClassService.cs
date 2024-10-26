using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IClassService
    {
        Task<ApiResponse<bool>> CreateClassAsync(CreateClassViewModel model);
        /* Task<ApiResponse<bool>> UpdateClassAsync(CreateClassViewModel update, string id);
         Task<ApiResponse<Pagination<ClassViewModel>>> FilterClassAsync(FilterClassViewModel filter);
         Task<ApiResponse<ClassViewModel>> GetClassByIdAsync(string id);
         Task<ApiResponse<bool>> UpdateClassStatusAsync(ClassStatusEnum status,string id);
         Task<ApiResponse<Pagination<ClassStudentViewModel>>> GetListStudentInClassAsync(string id);
         Task<ApiResponse<Pagination<StudentClassScheduleViewModel>>> GetClassSchedulesForCurrentUserByTermAsync(string termId, int pageIndex, int pageSize);
         Task<ApiResponse<bool>> DeleteClassAsync(string id);
         Task<ApiResponse<Pagination<ClassViewModel>>> GetClassesWithoutScheduleAsync(int pageIndex, int pageSize);*/
    }
}

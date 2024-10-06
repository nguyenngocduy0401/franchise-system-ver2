using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IClassScheduleService
    {
        Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel);
        Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync(CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel);
        Task<ApiResponse<bool>> DeleteClassScheduleByIdAsync(string id);
        Task<ApiResponse<Pagination<ClassScheduleViewModel>>> FilterClassScheduleAsync(FilterClassScheduleViewModel filterClassScheduleViewModel);
        Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleByIdAsync(string id);
        Task<ApiResponse<bool>> UpdateClassScheduleAsync(CreateClassScheduleViewModel updateClassScheduleViewModel, string id);
    }
}

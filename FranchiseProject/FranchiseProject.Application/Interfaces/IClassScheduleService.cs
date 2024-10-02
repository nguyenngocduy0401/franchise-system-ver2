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
        public Task<ApiResponse<bool>> CreateClassScheduleAsync(CreateClassScheduleViewModel createClassScheduleViewModel);
        public Task<ApiResponse<bool>> UpdateClassScheduleAsync(CreateClassScheduleViewModel updateClassScheduleViewModel);
        public Task<ApiResponse<ClassScheduleViewModel>> FilterClassScheduleAsync(FilterClassScheduleViewModel filterClassScheduleViewModel);
        public Task<ApiResponse<bool>> DeleteClassScheduleAsync(string id);
        public Task<ApiResponse<ClassScheduleViewModel>> GetClassScheduleAsync(string id);
        public Task<ApiResponse<bool>> CreateClassScheduleDateRangeAsync(CreateClassScheduleDateRangeViewModel createClassScheduleDateRangeViewModel);
    }
}

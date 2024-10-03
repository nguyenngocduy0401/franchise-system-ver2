using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModel;
using FranchiseProject.Application.ViewModels.TermViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface ITermService
    {
        Task<ApiResponse<bool>> CreateTermAsync(CreateTermViewModel createTermViewModel);
       
        Task<ApiResponse<bool>> DeleteTermByIdAsync(string id);
        Task<ApiResponse<Pagination<ClassScheduleViewModel>>> FilterTermAsync(FilterTermViewModel filterTermViewModel);
        Task<ApiResponse<ClassScheduleViewModel>> GetTermByIdAsync(string id);
        Task<ApiResponse<bool>> UpdateTermAsync(CreateTermViewModel createTermViewModel, string id);
    }
}

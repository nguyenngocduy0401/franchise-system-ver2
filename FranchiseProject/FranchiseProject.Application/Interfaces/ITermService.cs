using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.ClassScheduleViewModels;
using FranchiseProject.Application.ViewModels.TermViewModels;
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
        Task<ApiResponse<Pagination<TermViewModel>>> FilterTermAsync(FilterTermViewModel filterTermViewModel);
        Task<ApiResponse<TermViewModel>> GetTermByIdAsync(string id);
        Task<ApiResponse<bool>> UpdateTermAsync(CreateTermViewModel createTermViewModel, string id);
        Task<ApiResponse<Pagination<TermViewModel>>> GetAllTermAsync(int pageSize,int pageIndex);
    }
}

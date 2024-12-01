using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.HomePageViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Interfaces
{
    public interface IHomePageService
    {
        Task<ApiResponse<HomePageViewModel>> GetHomePage();
        Task<ApiResponse<bool>> UpdateHomePage(Guid id, UpdatePageModel updatePageModel);
    }
}

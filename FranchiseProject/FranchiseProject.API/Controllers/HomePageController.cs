using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Services;
using FranchiseProject.Application.ViewModels.HomePageViewModels;
using FranchiseProject.Application.ViewModels.QuestionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FranchiseProject.API.Controllers
{
    [Route("api/v1/home-pages")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageService _homePageService;
        public HomePageController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }
        [SwaggerOperation(Summary = "cập nhật trang chủ bằng id")]
        [HttpPut("{id}")]
        public async Task<ApiResponse<bool>> UpdateHomePage(Guid id, UpdatePageModel updatePageModel)
        {
            return await _homePageService.UpdateHomePage(id, updatePageModel);
        }
        [SwaggerOperation(Summary = "lấy thông trang chủ")]
        [HttpGet()]
        public async Task<ApiResponse<HomePageViewModel>> GetHomePage()
        {
            return await _homePageService.GetHomePage();
        }
    }
}

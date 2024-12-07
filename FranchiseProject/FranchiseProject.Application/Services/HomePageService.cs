using AutoMapper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.EMMA;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.HomePageViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;

        public HomePageService(IMapper mapper, IUnitOfWork unitOfWork,
            IClaimsService claimsService)
        {
            _claimsService = claimsService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<HomePageViewModel>> GetHomePage() 
        {
            var response = new ApiResponse<HomePageViewModel>();
            try
            {
                var homePage = (await _unitOfWork.HomePageRepository.GetAllAsync()).FirstOrDefault();
                var franchiseFee = (await _unitOfWork.FranchiseFeesRepository.GetAllAsync()).FirstOrDefault();

                var homePageModel = _mapper.Map<HomePageViewModel>(homePage);
                homePageModel.FeeAmount = (
                    franchiseFee != null  
                    && franchiseFee.FeeAmount != null 
                    && franchiseFee.FeeAmount > 0) ? franchiseFee.FeeAmount : 300000000;
                response = ResponseHandler.Success(homePageModel);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<HomePageViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> UpdateHomePage(Guid id, UpdatePageModel updatePageModel)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var homePage = await _unitOfWork.HomePageRepository.GetByIdAsync(id);
                if(homePage == null) return ResponseHandler.Success(false, "Cập nhật hiện không khả dụng!");
                homePage = _mapper.Map(updatePageModel, homePage);

                _unitOfWork.HomePageRepository.Update(homePage);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Updated failed!");

                response = ResponseHandler.Success(true);

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool> (ex.Message);
            }
            return response;
        }
    }
}

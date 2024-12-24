using AutoMapper;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.UserChapterMaterialViewModels;
using FranchiseProject.Domain.Entity;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class UserChapterMaterialService : IUserChapterMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly ICurrentTime _currentTime;
        private readonly IMapper _mappers;
        public UserChapterMaterialService(IUnitOfWork unitOfWork, IClaimsService claimsService,
            IMapper mapper, ICurrentTime currentTime)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _mappers = mapper;
            _currentTime = currentTime;
        }
        public async Task<ApiResponse<double>> GetCompletedPercentCourseAsync(string userId, Guid courseId)
        {
            var response = new ApiResponse<double>();
            try
            {
                if(userId == null) return ResponseHandler.Success((double)0);
                var completedPercent = await _unitOfWork.UserChapterMaterialRepository.CompletedPercentUserChapterMaterialAsync(courseId, userId);

                response = ResponseHandler.Success(completedPercent);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<double>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<UserChapterMaterialModel>> CreateUserChapterMaterialByLoginAsync(CreateUserChapterMaterialModel createUserChapterMaterial)
        { 
            var response = new ApiResponse<UserChapterMaterialModel>();
            try
            {
                string userId = _claimsService.GetCurrentUserId.ToString();
                if (userId == null) return ResponseHandler.Failure<UserChapterMaterialModel>("User not found");
                var filter = (Expression<Func<UserChapterMaterial, bool>>)
                    (e => e.ChapterMaterialId == createUserChapterMaterial.ChapterMaterialId && e.UserId == userId);

                var userChapterMaterial = _mappers.Map<UserChapterMaterial>(createUserChapterMaterial);
                userChapterMaterial.UserId = userId;

                var checkExist = await _unitOfWork.UserChapterMaterialRepository.AnyAsync(filter);
                if (checkExist == true) return ResponseHandler.Success(_mappers.Map<UserChapterMaterialModel>(userChapterMaterial), "Đánh dấu đã đọc thành công!");

                await _unitOfWork.UserChapterMaterialRepository.AddAsync(userChapterMaterial);
                userChapterMaterial.CompletedDate = _currentTime.GetCurrentTime();
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");
                response = ResponseHandler.Success(_mappers.Map<UserChapterMaterialModel>(userChapterMaterial), "Đánh dấu đã đọc thành công!");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<UserChapterMaterialModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<UserChapterMaterialModel>> GetUserChapterMaterialByLoginAsync(Guid userChapterMaterialId)
        {
            var response = new ApiResponse<UserChapterMaterialModel>();
            try
            {
                string userId = _claimsService.GetCurrentUserId.ToString();
                if (userId == null)
                {
                    return ResponseHandler.Failure<UserChapterMaterialModel>("User not found");
                }
                var userChapterMaterial = (await _unitOfWork.UserChapterMaterialRepository.FindAsync(x => x.ChapterMaterialId == userChapterMaterialId && x.UserId == userId))
                    .FirstOrDefault();
                var userChapterMaterialModel = _mappers.Map<UserChapterMaterialModel>(userChapterMaterial);

                response = ResponseHandler.Success(userChapterMaterialModel);
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<UserChapterMaterialModel>(ex.Message);
            }
            return response;
        }
    }
}

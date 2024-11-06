using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AssignmentViewModels;
using FranchiseProject.Application.ViewModels.FeedBackViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateFeedBackViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public FeedbackService(IEmailService emailService, IHubContext<NotificationHub> hubContext, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateFeedBackViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
            _hubContext = hubContext;
            _emailService = emailService;

        }
        #endregion

        public async Task<ApiResponse<bool>> CreateFeedBackAsync(CreateFeedBackViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Failure<bool>("User hoặc Agency không khả dụng!");
                }
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(model);
                if (!validationResult.IsValid)
                {
                    return ValidatorHandler.HandleValidation<bool>(validationResult);

                }
                var rc = _unitOfWork.RegisterCourseRepository.GetFirstOrDefaultAsync(rc => rc.UserId == userCurrentId && rc.CourseId == Guid.Parse(model.CourseId) && rc.StudentCourseStatus == StudentCourseStatusEnum.Studied);
                if (rc == null)
                {
                    return ResponseHandler.Success<bool>(false,"Học sinh chưa hoàn thành khóa học !");

                }
                var feedback = _mapper.Map<Feedback>(model);
                await _unitOfWork.FeedbackRepository.AddAsync(feedback);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Create failed!");

                response = ResponseHandler.Success(true, "Tạo đánh giá thành công!");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<FeedBackViewModel>> GetFeedBaByIdAsync(Guid feedbackId)
        {

            var response = new ApiResponse<FeedBackViewModel>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<FeedBackViewModel>(null,"User hoặc Agency không khả dụng!");
                }
                var feedback = await _unitOfWork.FeedbackRepository.GetByIdAsync(feedbackId);
                if (feedback == null) throw new Exception("đánh giá không tồn tại!");
                var slotViewModel = _mapper.Map<FeedBackViewModel>(feedback);
                response = ResponseHandler.Success(slotViewModel, "Lấy thông tin slot thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<FeedBackViewModel>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> DeleteFeedBackByIdAsync(Guid id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var feedBack = await _unitOfWork.FeedbackRepository.GetExistByIdAsync(  id);
                if (feedBack == null) return ResponseHandler.Success<bool>(false,"Feedback học không khả dụng!");

                _unitOfWork.FeedbackRepository.HardRemove(feedBack);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess) throw new Exception("Delete failed!");

                response = ResponseHandler.Success(true, "Xoá Feedback học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
       public async Task<ApiResponse<Pagination<FeedBackViewModel>>> FilterFeedBackAsync(FilterFeedbackViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<FeedBackViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
             
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());

                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Failure<Pagination<FeedBackViewModel>>("User hoặc Agency không khả dụng!");
                }
                Expression<Func<Feedback, bool>> filter = f =>
                (string.IsNullOrEmpty(filterModel.CourseId) || f.CourseId.ToString() == filterModel.CourseId) &&
                    f.User != null && f.User.AgencyId == userCurrent.AgencyId;
                var feedbacks = await _unitOfWork.FeedbackRepository.GetFilterAsync(
                filter: filter,
                pageIndex: filterModel.PageIndex,
                    pageSize: filterModel.PageSize
                );
                var feedbackViewModels = _mapper.Map<Pagination<FeedBackViewModel>>(feedbacks);
                if (feedbackViewModels.Items.IsNullOrEmpty())
                    return ResponseHandler.Success(feedbackViewModels, "Không tìm thấy feedback phù hợp!");

                response = ResponseHandler.Success(feedbackViewModels, "Successful!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<FeedBackViewModel>>(ex.Message);
            }
            return response;
        }
    }
}

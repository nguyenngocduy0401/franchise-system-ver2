using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Services
{
    public class RegisterFormService :IRegisterFormSevice
    {
        private readonly IClaimsService _claimsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<RegisterConsultationViewModel> _validator;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public RegisterFormService(IEmailService emailService,IUnitOfWork unitOfWork,IClaimsService claimsService, IValidator<RegisterConsultationViewModel> validator,
            IMapper mapper,UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
                _claimsService = claimsService;
                _validator = validator;
            _mapper = mapper; 
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<ApiResponse<bool>> RegisterConsultationAsync(RegisterConsultationViewModel regis)
        {
            var response = new ApiResponse<bool>();
            try
            {
                FluentValidation.Results.ValidationResult validationResult = await _validator.ValidateAsync(regis);
                if (!validationResult.IsValid)
                {
                    response.isSuccess = false;
                    response.Message = string.Join(", ", validationResult.Errors.Select(error => error.ErrorMessage));
                    return response;
                }
                var twentyFourHoursAgo = DateTime.Now.AddHours(-24);
                bool existsWithin24Hours = await _unitOfWork.FranchiseRegistrationRequestRepository.ExistsWithinLast24HoursAsync(regis.Email, regis.PhoneNumber);

                if (existsWithin24Hours)
                {
                    response.isSuccess = false;
                    response.Message = "Bạn đã đăng ký tư vấn trong vòng 24 giờ qua. Vui lòng thử lại sau.";
                    return response;
                }

                var franchiseRequest = _mapper.Map<RegisterForm>(regis);
                franchiseRequest.Status = ConsultationStatusEnum.NotConsulted;
                franchiseRequest.CustomerName = regis.CustomerName;
                franchiseRequest.ModificationDate = DateTime.Now;
                await _unitOfWork.FranchiseRegistrationRequestRepository.AddAsync(franchiseRequest);
                var isSuccess = await _unitOfWork.SaveChangeAsync();
               if (isSuccess > 0)
                {
                    var emailResponse = await _emailService.SendRegistrationSuccessEmailAsync(regis.Email); 
                    if (!emailResponse.isSuccess)
                    {
                     response.Message = "Tạo Thành Công, nhưng không thể gửi email xác nhận.";
                    }
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Tạo Thành Công !";
                }
                else
                {
                    throw new Exception("Create unsuccessfully");
                }
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
          
        }

        public async Task<ApiResponse<bool>> UpdateConsultationStatusAsync(string requestId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userId= _claimsService.GetCurrentUserId.ToString();
                
                var id= Guid.Parse(requestId);
                var exist = await _unitOfWork.FranchiseRegistrationRequestRepository.GetByIdAsync(id);
                if (exist == null)
                {
                 
                    response.Data = false;
                    response.isSuccess = true;
                    response.Message = "không tìm thấy";

                }
                exist.Status = ConsultationStatusEnum.Consulted;
                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser == null)
                {
                    response.Data=false;
                    response.isSuccess = true;
                    response.Message = "Current user not found.";
                    
                }
               exist.ConsultanId = userId;
               _unitOfWork.FranchiseRegistrationRequestRepository.Update(exist);
                var result = await _unitOfWork.SaveChangeAsync();
                if (result > 0)
                {
                    response.Data = true;
                    response.isSuccess = true;
                    response.Message = "Cập nhật trạng thái thành công.";
                }
                else
                {
                    throw new Exception("Update failed!");
                }
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<ConsultationViewModel>>> FilterConsultationAsync(FilterConsultationViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<ConsultationViewModel>>();

            try
            {
                var paginationResult = await _unitOfWork.FranchiseRegistrationRequestRepository.GetFilterAsync(
                    filter: s => (!filterModel.Status.HasValue || s.Status == filterModel.Status),
                    includeProperties: "User",
                    pageIndex: filterModel.PageIndex,
                    pageSize: filterModel.PageSize
                );

                if (paginationResult?.Items == null)
                {
                    return ResponseHandler.Failure<Pagination<ConsultationViewModel>>("No items found.");
                }

                var consultantIds = paginationResult.Items
                    .Where(item => item?.ConsultanId != null)
                    .Select(item => item.ConsultanId)
                    .Distinct()
                    .ToList();

                var consultants = await _userManager.Users
                    .Where(u => consultantIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.UserName);

                var consultationViewModels = paginationResult.Items.OrderByDescending(item => item.ModificationDate).Select(item => new ConsultationViewModel
                {
                    Id = item.Id,
                    CusomterName = item.CustomerName,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Status = item.Status,
                    ModificationDate = item.ModificationDate,
                    ConsultantUserName = item.ConsultanId != null && consultants.TryGetValue(item.ConsultanId, out var userName)
                        ? userName
                        : null
                }).ToList();

                var paginationViewModel = new Pagination<ConsultationViewModel>
                {
                    PageIndex = paginationResult.PageIndex,
                    PageSize = paginationResult.PageSize,
                    TotalItemsCount = paginationResult.TotalItemsCount,
                    Items = consultationViewModels
                };

                response.Data = paginationViewModel;
                response.isSuccess = true;
                response.Message = "Truy xuất thành công";
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<ConsultationViewModel>> GetByIdAsync(Guid id)
        {
            var response = new ApiResponse<ConsultationViewModel>();

            try
            {
              
                var franchiseRequest = await _unitOfWork.FranchiseRegistrationRequestRepository.GetByIdAsync(id);

                if (franchiseRequest == null)
                {
                    response.isSuccess = false;
                    response.Message = "Franchise registration request not found.";
                    return response;
                }
                var result = _mapper.Map<ConsultationViewModel>(franchiseRequest);

                response.isSuccess = true;
                response.Data = result;
            }
            catch (DbException ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;

            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

using AutoMapper;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.SlotViewModels;

using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
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
    public class PaymentService : IPaymentService
    {
        #region Contructor
        private readonly IUnitOfWork _unitOfWork;
        private readonly  IMapper _mapper;
        private readonly  IValidator<CreateStudentPaymentViewModel> _validator;
        private readonly  IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        public PaymentService(IUserService userService,UserManager<User> userManager,IUnitOfWork unitOfWork,IMapper mapper,IValidator<CreateStudentPaymentViewModel>validator
            ,IEmailService emailService,IHubContext<NotificationHub>hubContext,IClaimsService claimsService)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _hubContext = hubContext;
            _emailService = emailService;
            _userManager = userManager;
            _userService = userService;
        }
        #endregion
        public async Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create,  StudentPaymentStatusEnum status )
        {
            var response = new ApiResponse<bool>();
            try
            {
                var rc = await _unitOfWork.RegisterCourseRepository.GetExistByIdAsync(Guid.Parse(create.RegisterCourseId));
                var userCurrentId =  _claimsService.GetCurrentUserId.ToString();
                var userCurrent =await _userManager.FindByIdAsync(userCurrentId);
                switch (status)
                {
                    case StudentPaymentStatusEnum.Advance_Payment:
                        if (rc.StudentCourseStatus == StudentCourseStatusEnum.Pending) {
                            var payment = _mapper.Map<Payment>(create);
                            rc.StudentPaymentStatus=StudentPaymentStatusEnum.Advance_Payment;
                            rc.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;
                            payment.UserId = rc.UserId;
                            await _unitOfWork.RegisterCourseRepository.UpdateAsync(rc);
                            await _unitOfWork.PaymentRepository.AddAsync(payment);
                            var user = await _userManager.FindByIdAsync(rc.UserId);
                            var generate = await _userService.GenerateUserCredentials(user.FullName);
                            user.UserName = generate.UserName;
                            user.Status = UserStatusEnum.active;
                            user.AgencyId = userCurrent.AgencyId;
                            await _userManager.AddToRoleAsync(user, AppRole.Student);

                            await _userManager.AddPasswordAsync(user, generate.Password);
                            var result = await _userManager.UpdateAsync(user);

                            if (!result.Succeeded)
                            {
                                throw new Exception("Update User Account fail!");
                            }
                            var email = EmailTemplate.StudentPaymentSuccsess(user.Email, user.FullName, create.Amount, generate.UserName, generate.Password);
                          var mailSuccess= await _emailService.SendEmailAsync(email);
                           
                        }
                        break;
                    case StudentPaymentStatusEnum.Completed:
                        if (rc.StudentCourseStatus == StudentCourseStatusEnum.Pending || rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted || rc.StudentCourseStatus == StudentCourseStatusEnum.Enrolled)
                        {
                            var payment = _mapper.Map<Payment>(create);
                            rc.StudentPaymentStatus = StudentPaymentStatusEnum. Completed;
                            rc.StudentCourseStatus = StudentCourseStatusEnum.Waitlisted;
                            payment.UserId = rc.UserId;
                            await _unitOfWork.RegisterCourseRepository.UpdateAsync(rc);
                            await _unitOfWork.PaymentRepository.AddAsync(payment);
                            var user = await _userManager.FindByIdAsync(rc.UserId);
                            var generate = await _userService.GenerateUserCredentials(user.FullName);
                            user.UserName = generate.UserName;
                            user.Status = UserStatusEnum.active;
                            user.AgencyId = userCurrent.AgencyId;
                            await _userManager.AddToRoleAsync(user, AppRole.Student);

                            await _userManager.AddPasswordAsync(user, generate.Password);
                            var result = await _userManager.UpdateAsync(user);

                            if (!result.Succeeded)
                            {
                                throw new Exception("Update User Account fail!");
                            }
                            var email = EmailTemplate.StudentPaymentSuccsess(user.Email, user.FullName, create.Amount, generate.UserName, generate.Password);
                            var mailSuccess = await _emailService.SendEmailAsync(email);
                        }
                        break;


                       
                };
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
             
                return ResponseHandler.Success<bool>(true, "Tạo thanh toán thành công !");

            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<Pagination<PaymentStudentViewModel>>> FilterPaymentAsync(FilterStudentPaymentViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<PaymentStudentViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var currentAgencyId = userCurrent.AgencyId;
                var usersInAgency = await _unitOfWork.UserRepository.GetAllAsync(u =>
                    u.AgencyId == currentAgencyId &&
                    (string.IsNullOrEmpty(filterModel.StudentName) || u.FullName.Contains(filterModel.StudentName))
                );

                var userIdsInAgency = usersInAgency.Select(u => u.Id).ToList();
                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(p =>
                    userIdsInAgency.Contains(p.UserId)
                );
                var paymentViewModels = _mapper.Map<List<PaymentStudentViewModel>>(payments);
                var pagedResult = new Pagination<PaymentStudentViewModel>
                {
                    TotalItemsCount = paymentViewModels.Count,
                    PageSize = filterModel.PageSize,
                    PageIndex = filterModel.PageIndex,
                    Items = paymentViewModels
                        .Skip((filterModel.PageIndex - 1) * filterModel.PageSize)
                        .Take(filterModel.PageSize)
                        .ToList()
                };

                response = ResponseHandler.Success(pagedResult, "Lọc thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentStudentViewModel>>(ex.Message);
            }
            return response;
        }



        public async Task<ApiResponse<PaymentStudentViewModel>> GetPaymentByIdAsync(string paymentId)
        {
            var response = new ApiResponse<PaymentStudentViewModel>();
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(Guid.Parse (paymentId));
                if (payment == null)
                {
                    return ResponseHandler.Failure<PaymentStudentViewModel>("Không tìm thấy thanh toán với ID này!");
                }
                var paymentViewModel = _mapper.Map<PaymentStudentViewModel>(payment);

                response = ResponseHandler.Success(paymentViewModel, "Lấy thông tin thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<PaymentStudentViewModel>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<Pagination<PaymentStudentViewModel>>> GetPaymentByLoginAsync(int pageIndex = 1, int pageSize = 10)
        {
            var response = new ApiResponse<Pagination<PaymentStudentViewModel>>();
            try
            {
              
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();

                var payments = await _unitOfWork.PaymentRepository.GetAllAsync(p => p.UserId == userCurrentId);

            
                var paymentViewModels = _mapper.Map<List<PaymentStudentViewModel>>(payments);

          
                var pagedResult = new Pagination<PaymentStudentViewModel>
                {
                    TotalItemsCount = paymentViewModels.Count,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = paymentViewModels.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                response = ResponseHandler.Success(pagedResult, "Lấy danh sách thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentStudentViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UpdateStudentPaymentStatusAsync(Guid registerCourseId, StudentPaymentStatusEnum newStatus)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetByIdAsync(registerCourseId);
                if (registerCourse == null)
                {
                    return ResponseHandler.Success<bool>(false,"Không tìm thấy thông tin khóa học.");
                }
                var currentDate = DateTime.UtcNow;
                if (registerCourse.PaymentDeadline.HasValue && currentDate <= registerCourse.PaymentDeadline)
                {
                    return ResponseHandler.Success<bool>(false, "Chưa đến hạn thanh toán!");
                }
                if (registerCourse.StudentPaymentStatus==StudentPaymentStatusEnum.Completed)
                {
                    return ResponseHandler.Success<bool>(false, "Học sinh đã hoàn thành thanh toán!");
                }
                registerCourse.StudentPaymentStatus = newStatus;
                await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
             //   if (!isSuccess) throw new Exception("Cập nhật thất bại!");
                response = ResponseHandler.Success(true, "Cập nhật trạng thái thanh toán thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<bool>($"Lỗi khi cập nhật trạng thái thanh toán: {ex.Message}");
            }
            return response;
        }


    }
}

﻿using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Hubs;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Utils;
using FranchiseProject.Application.ViewModels.ConsultationViewModels;
using FranchiseProject.Application.ViewModels.ContractViewModels;
using FranchiseProject.Application.ViewModels.PaymentViewModel;
using FranchiseProject.Application.ViewModels.PaymentViewModel.PaymentContractViewModels;
using FranchiseProject.Application.ViewModels.SlotViewModels;

using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
        private readonly IMapper _mapper;
        private readonly IValidator<CreateStudentPaymentViewModel> _validator;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IVnPayService _vnPayService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PaymentService(IServiceScopeFactory serviceScopeFactory,IUserService userService, UserManager<User> userManager, IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateStudentPaymentViewModel> validator
            , IEmailService emailService, IHubContext<NotificationHub> hubContext, IClaimsService claimsService, IVnPayService vnPayService)
        {
            _claimsService = claimsService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _hubContext = hubContext;
            _emailService = emailService;
            _userManager = userManager;
            _userService = userService;
            _vnPayService = vnPayService;
            _serviceScopeFactory= serviceScopeFactory;
        }
        #endregion
        public async Task<ApiResponse<bool>> CreatePaymentStudent(CreateStudentPaymentViewModel create, StudentPaymentStatusEnum status)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var rc = await _unitOfWork.RegisterCourseRepository.GetExistByIdAsync(Guid.Parse(create.RegisterCourseId));
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId);
                switch (status)
                {
                    case StudentPaymentStatusEnum.Advance_Payment:
                        if (rc.StudentCourseStatus == StudentCourseStatusEnum.Pending)
                        {
                            var payment = _mapper.Map<Payment>(create);
                            rc.StudentPaymentStatus = StudentPaymentStatusEnum.Advance_Payment;
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
                    case StudentPaymentStatusEnum.Completed:
                        if (rc.StudentCourseStatus == StudentCourseStatusEnum.Pending || rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted || rc.StudentCourseStatus == StudentCourseStatusEnum.Enrolled)
                        {
                            var payment = _mapper.Map<Payment>(create);
                            rc.StudentPaymentStatus = StudentPaymentStatusEnum.Completed;
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
        public async Task<ApiResponse<bool>> CreatePaymentContractDirect(CreateContractDirect create)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(create.AgencyId);

                var isPay = await _unitOfWork.ContractRepository.IsDepositPaidCorrectlyAsync(contract.Id);
                if (isPay)
                {

                    return ResponseHandler.Success<bool>(false, "Hợp đồng đã được thanh toán lần 1!");

                }
                else
                {
                    var payment = new Payment
                    {
                        Title = "Thanh toán hợp đồng nhượn quyền" + " lần 1 " + contract.ContractCode,
                        Description = create.Description,
                        Amount = contract.Total * (contract.DepositPercentage / 100),
                        Type = PaymentTypeEnum.Contract,
                        Method = PaymentMethodEnum.Direct,
                        Status = PaymentStatus.Completed,
                        ImageURL = create.ImageUrl,
                        ContractId = contract.Id,
                        UserId = userId.ToString()
                    };
                    await _unitOfWork.PaymentRepository.AddAsync(payment);
                    contract.PaidAmount = payment.Amount;
                    _unitOfWork.ContractRepository.Update(contract);
                    var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                    return ResponseHandler.Success<bool>(true, "Tạo thanh toán thành công !");
                }

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
                var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(Guid.Parse(paymentId));
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
                    return ResponseHandler.Success<bool>(false, "Không tìm thấy thông tin khóa học.");
                }
                var currentDate = DateTime.UtcNow;
                if (registerCourse.PaymentDeadline.HasValue && currentDate <= registerCourse.PaymentDeadline)
                {
                    return ResponseHandler.Success<bool>(false, "Chưa đến hạn thanh toán!");
                }
                if (registerCourse.StudentPaymentStatus == StudentPaymentStatusEnum.Completed)
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

        public async Task<ApiResponse<Pagination<PaymentContractAgencyViewModel>>> FilterPaymentContractAsync(FilterContractPaymentViewModel filterModel)
        {
            var response = new ApiResponse<Pagination<PaymentContractAgencyViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());


                Expression<Func<Payment, bool>> filter = p =>
                    p.Type == PaymentTypeEnum.Contract &&
                    (filterModel.AgencyId == null || p.Contract.AgencyId == filterModel.AgencyId) &&
                    (filterModel.StartDate == null || p.CreationDate >= filterModel.StartDate) &&
                    (filterModel.EndDate == null || p.CreationDate <= filterModel.EndDate);


                var payments = await _unitOfWork.PaymentRepository.GetFilterAsync(
                    filter: filter,
                    includeProperties: "Contract,Contract.Agency",
                    pageIndex: filterModel.PageIndex,
                    pageSize: filterModel.PageSize
                );

                var paymentViewModels = new Pagination<PaymentContractAgencyViewModel>
                {
                    Items = payments.Items.Select(p => new PaymentContractAgencyViewModel
                    {
                        Id = p.Id,
                        ContractId = p.ContractId,
                        ContractCode = p.Contract.ContractCode,
                        AgencyName = p.Contract.Agency.Name,
                        Amount = p.Amount,
                        Status = p.Status,
                        CreationDate = p.CreationDate,
                        ImageURL = p.ImageURL,
                        Description = p.Description,
                        Method = p.Method,
                        Title = p.Title,
                        CreateBy = p.UserId != null ? _userManager.FindByIdAsync(p.UserId).Result?.UserName : null
                    }).ToList(),
                    TotalItemsCount = payments.TotalItemsCount,
                    PageIndex = payments.PageIndex,
                    PageSize = payments.PageSize
                };

                response = ResponseHandler.Success(paymentViewModels, "Lọc thanh toán hợp đồng thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentContractAgencyViewModel>>(ex.Message);
            }
            return response;
        }
        public async Task<ApiResponse<bool>> CreateRefundPayment(CreateRefundPaymentViewModel model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var userId = _claimsService.GetCurrentUserId.ToString();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ResponseHandler.Failure<bool>("User not found.");
                }

                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetByIdAsync(model.RegisterCourseId);
                if (registerCourse == null)
                {
                    return ResponseHandler.Failure<bool>("Register Course payment not found.");
                }

                decimal refundAmount = 0;
                var classRoom = await _unitOfWork.ClassRoomRepository.GetFirstOrDefaultAsync(x => x.UserId == registerCourse.UserId && x.Class.CourseId == registerCourse.CourseId);
                if (classRoom == null)
                {
                    return ResponseHandler.Failure<bool>("Class room not found for the student.");
                }

                var classE = await _unitOfWork.ClassRepository.GetExistByIdAsync(classRoom.ClassId.Value);
                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(registerCourse.CourseId.Value);
                if (course == null)
                {
                    return ResponseHandler.Failure<bool>("Course not found.");
                }

                if (classE != null)
                {
                    var classStartDate = await _unitOfWork.ClassScheduleRepository.GetEarliestClassScheduleByClassIdAsync(classRoom.ClassId.Value);
                    if (classStartDate != null && classStartDate.Date.HasValue)
                    {
                        var currentDate = DateTime.Now;
                        if (currentDate >= classStartDate.Date.Value)
                        {
                            return ResponseHandler.Success<bool>(false, "Lớp học đã bắt đầu không thể hoàn tiền!");
                        }
                        var daysUntilStart = (classStartDate.Date.Value - DateTime.Now).TotalDays;

                        if (daysUntilStart > 10)
                        {
                            refundAmount = course.Price.GetValueOrDefault(); // 80% refund
                        }
                        else if (daysUntilStart < 10 && daysUntilStart > 5)
                        {
                            refundAmount = course.Price.GetValueOrDefault() * 0.8m;
                        }
                        else if (daysUntilStart < 5)
                        {
                            refundAmount = course.Price.GetValueOrDefault() * 0.5m; // 50% refund
                        }
                        else
                        {
                            return ResponseHandler.Failure<bool>("Refund is not possible within 5 days of class start.");
                        }
                    }
                    else
                    {
                        return ResponseHandler.Failure<bool>("Class start date not found.");
                    }
                }
                else
                {

                    refundAmount = course.Price.GetValueOrDefault();
                }
                var paymentsRc = await _unitOfWork.PaymentRepository.GetPaymentByRegisterCourseIdAndUserId(model.RegisterCourseId, registerCourse.UserId);

                
                if (paymentsRc == null)
                {
                    return ResponseHandler.Failure<bool>("Original payment not found.");
                }
                var refundPayment = new Payment
                {
                    Title = $"Hoàn tiền khóa học {registerCourse.Course.Name}",
                    Description = model.RefundReason,
                    Amount = (double)refundAmount,
                    Type = PaymentTypeEnum.Refund,
                    Method = PaymentMethodEnum.Direct,
                    Status = PaymentStatus.Completed,
                    CreationDate = DateTime.UtcNow,
                    UserId = registerCourse.UserId,
                    RegisterCourseId = model.RegisterCourseId,
                    AgencyId = user.AgencyId,
                    ImageURL = model.ImageUrl,
                    ToDate= paymentsRc.ToDate ,
                };

                await _unitOfWork.PaymentRepository.AddAsync(refundPayment);

                //  RegisterCourse 
                registerCourse.StudentPaymentStatus = StudentPaymentStatusEnum.Refund;
                registerCourse.StudentCourseStatus = StudentCourseStatusEnum.Cancel;
                await _unitOfWork.RegisterCourseRepository.UpdateAsync(registerCourse);
                //class 
                classE.CurrentEnrollment--;
                 _unitOfWork.ClassRepository.Update(classE);
                //  ClassRoom
                await _unitOfWork.ClassRoomRepository.DeleteAsync(classRoom);

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;

                var student = await _userManager.FindByIdAsync(registerCourse.UserId);
                if (student != null)
                {
                    var emailMessage = EmailTemplate.RefundConfirmationEmail(
                        student.Email,
                        student.FullName,
                        registerCourse.Course.Name,
                        refundAmount,
                        model.RefundReason
                    );

                    var emailSent = await _emailService.SendEmailAsync(emailMessage);
                }
                return ResponseHandler.Success(true, "Hoàn tiền thành công.");


            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<bool>($"An error occurred while creating the refund payment: {ex.Message}");
            }
        }
        public async Task<ApiResponse<decimal>> CalculateRefundAmount(Guid registerCourseId)
        {
            try
            {
                var registerCourse = await _unitOfWork.RegisterCourseRepository.GetByIdAsync(registerCourseId);
                if (registerCourse == null)
                {
                    return ResponseHandler.Failure<decimal>("Register Course not found.");
                }

                var course = await _unitOfWork.CourseRepository.GetExistByIdAsync(registerCourse.CourseId.Value);
                if (course == null)
                {
                    return ResponseHandler.Failure<decimal>("Course not found.");
                }

                var classRoom = await _unitOfWork.ClassRoomRepository.GetFirstOrDefaultAsync(x => x.UserId == registerCourse.UserId && x.Class.CourseId == registerCourse.CourseId);
                if (classRoom == null)
                {
                    return ResponseHandler.Failure<decimal>("Class room not found for the student.");
                }

                decimal refundAmount = 0;
                var classE = await _unitOfWork.ClassRepository.GetExistByIdAsync(classRoom.ClassId.Value);

                if (classE != null)
                {
                    var classStartDate = await _unitOfWork.ClassScheduleRepository.GetEarliestClassScheduleByClassIdAsync(classRoom.ClassId.Value);
                    if (classStartDate != null && classStartDate.Date.HasValue)
                    {
                        var currentDate = DateTime.Now;
                        if (currentDate >= classStartDate.Date.Value)
                        {
                            return ResponseHandler.Success(0m, "Lớp học đã bắt đầu không thể hoàn tiền!");
                        }
                        var daysUntilStart = (classStartDate.Date.Value - DateTime.Now).TotalDays;

                        if (daysUntilStart > 10)
                        {
                            refundAmount = course.Price.GetValueOrDefault() ; // 80% refund
                        }
                        else  if(daysUntilStart<10 && daysUntilStart > 5)
                        {
                            refundAmount = course.Price.GetValueOrDefault() * 0.8m;
                        }
                        else if (daysUntilStart < 5)
                        {
                            refundAmount = course.Price.GetValueOrDefault() * 0.5m; // 50% refund
                        }
                        else
                        {
                            return ResponseHandler.Success(0m, "Refund is not possible within 5 days of class start.");
                        }
                    }
                    else
                    {
                        return ResponseHandler.Failure<decimal>("Class start date not found.");
                    }
                }
                else
                {
                    refundAmount = course.Price.GetValueOrDefault();
                }

                return ResponseHandler.Success(refundAmount, "Refund amount calculated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<decimal>($"An error occurred while calculating the refund amount: {ex.Message}");
            }
        }
        //---------------Thanh toán hàng tháng -----------------------------------
        public async Task<ApiResponse<string>> CreateMonthlyRevenueSharePayment(CreatePaymentMontlyViewModel model)
        {
            try
            {
             
                if (  model.AgencyId == null || model.Amount == null)
                {
                    return ResponseHandler.Failure<string>("ContractId, AgencyId, and Amount are required.");
                }

             
                var agency = await _unitOfWork.AgencyRepository.GetByIdAsync(model.AgencyId.Value);

                if ( agency == null)
                {
                    return ResponseHandler.Failure<string>("Contract or Agency not found.");
                }

                var payment = new Payment
                {
                    Title = model.Title ?? $"Thanh toán phí chia sẻ doanh thu tháng - {DateTime.Now:MMMM yyyy}-{agency.Name}",
                    Description = model.Description ?? $"Thanh toán phí chia sẻ doanh thu tháng - {DateTime.Now:MMMM yyyy}-{agency.Name}",
                    Amount = model.Amount.Value,
                    Type = PaymentTypeEnum.MonthlyDue,
                    Method = PaymentMethodEnum.BankTransfer,
                    Status = PaymentStatus.NotCompleted,
                    AgencyId = model.AgencyId,
                    CreationDate = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddDays(5)
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);

                var paymentUrl = await CreateOrRefreshPaymentUrl(payment);

                await _unitOfWork.SaveChangeAsync();

                return ResponseHandler.Success(paymentUrl, "Monthly revenue share payment created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseHandler.Failure<string>($"An error occurred: {ex.Message}");
            }
        }
        public async Task<ApiResponse<Pagination<PaymentMonthlydueViewModel>>> FilterPaymentMonthlyDueAsync(FilterPaymentMonthlyDueModel filterModel)
        {
            var response = new ApiResponse<Pagination<PaymentMonthlydueViewModel>>();
            try
            {
                Expression<Func<Payment, bool>> filter = p =>
                    p.Type == PaymentTypeEnum.MonthlyDue &&
                    (filterModel.AgencyId == null || p.AgencyId == filterModel.AgencyId) &&
                    (filterModel.StartDate == null || p.CreationDate >= filterModel.StartDate) &&
                    (filterModel.EndDate == null || p.CreationDate <= filterModel.EndDate) &&
                    (filterModel.Status == null || p.Status == filterModel.Status);

                var payments = await _unitOfWork.PaymentRepository.GetFilterAsync(
                    filter: filter,
                    pageIndex: filterModel.PageIndex,
                    orderBy: q => q.OrderByDescending(p => p.CreationDate),
                    pageSize: filterModel.PageSize
                );

                var paymentViewModels = new List<PaymentMonthlydueViewModel>();

                foreach (var p in payments.Items)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        DateTime creationDate = p.CreationDate;
                        DateTime newDate = creationDate.AddMonths(-1);
                        var month = newDate.Month;
                        var year = newDate.Year;
                        var startDate = new DateTime(year, month, 1);
                        var endDate = startDate.AddMonths(1).AddDays(-1);

                        var monthlyRevenue = await scopedUnitOfWork.PaymentRepository.CalculateAgencyRevenue(p.AgencyId.Value, startDate, endDate);
                        var monthlyRefunds = await scopedUnitOfWork.PaymentRepository.GetTotalRefundsForAgencyInPeriod(p.AgencyId.Value, startDate, endDate);
                        var contract = await scopedUnitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(p.AgencyId.Value);
                        var revenueSharePercentage = contract.RevenueSharePercentage;
                        var actualProfits = (monthlyRevenue - monthlyRefunds) * (1 - revenueSharePercentage / 100);

                        paymentViewModels.Add(new PaymentMonthlydueViewModel
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Description = p.Description,
                            Amount = p.Amount,
                            Status = p.Status,
                            AgencyId = p.AgencyId,
                            PaymentUrl = p.PaymentUrl,
                            PaidDate = p.Status == PaymentStatus.Completed ? p.PaidDate : null,
                            CreattionDate = p.CreationDate,
                            Revenue = monthlyRevenue,
                            RevenueSharePercentage = revenueSharePercentage,
                            ActualProfits = actualProfits,
                            Refunds = monthlyRefunds
                        });
                    }
                }

                var paginatedResult = new Pagination<PaymentMonthlydueViewModel>
                {
                    Items = paymentViewModels,
                    TotalItemsCount = payments.TotalItemsCount,
                    PageIndex = payments.PageIndex,
                    PageSize = payments.PageSize
                };

                response = ResponseHandler.Success(paginatedResult, "Lọc thanh toán hàng tháng thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<Pagination<PaymentMonthlydueViewModel>>(ex.Message);
            }
            return response;
        }

        public async Task<string> CreateOrRefreshPaymentUrl(Payment payment)
        {
            var paymentUrl = await _vnPayService.CreatePaymentUrlForCourse(payment.AgencyId.Value, payment.Amount.Value,payment.Id );
            payment.PaymentUrl = paymentUrl;
            payment.LastUrlGenerationTime = DateTime.Now;
            return paymentUrl;
        }
    }
}

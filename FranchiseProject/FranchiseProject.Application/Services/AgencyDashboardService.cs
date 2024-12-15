using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
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
    public class AgencyDashboardService :IAgencyDashboardService
    {
        #region Constructor
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly IValidator<CreateClassViewModel> _validator;
        private readonly IValidator<UpdateClassViewModel> _validatorUpdate;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public AgencyDashboardService(IValidator<UpdateClassViewModel> validatorUpdate, IMapper mapper, IUnitOfWork unitOfWork, IClaimsService claimsService, IValidator<CreateClassViewModel> validator, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _validator = validator;
            _roleManager = roleManager;
            _userManager = userManager;
            _validatorUpdate = validatorUpdate;
        }
        #endregion
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<List<CourseRevenueViewModel>>();
            try
            {
                var userCurrentId = _claimsService.GetCurrentUserId.ToString();
                var userCurrent =await _userManager.FindByIdAsync(userCurrentId);
                var agencyId = userCurrent.AgencyId;
                var courseRevenueList = new List<CourseRevenueViewModel>();
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(agencyId.Value);
                var courseList = await _unitOfWork.CourseRepository.GetAllAsync();
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId.Value);
                foreach (var course in courseList)
                {
                    var filteredRegisterCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCourseByCourseIdAsync(course.Id);
                    var TotalMoney = (double)0;
                    var studentCount = 0; 
                    var validRegistrations = filteredRegisterCourses
                .Where(r => r.CreationDate.Date <= endDate.Date)
                .ToList();
                    foreach (var registration in validRegistrations)
                    {
                        studentCount++;
                          var payments=await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                       foreach (var payment in payments)
                        {
                            if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                            {
                                TotalMoney += payment.Amount ?? 0;
                            }
                        }
                    }
                    courseRevenueList.Add(new CourseRevenueViewModel
                    {
                        CourseId= course.Id,
                        StudentCount= studentCount,
                        CourseCode=course.Code,
                        CourseName=course.Name,
                        TotalRevenue=TotalMoney,
                        MonthlyFee= (double)(TotalMoney *contract.RevenueSharePercentage)
                    });
                }
                response = ResponseHandler.Success(courseRevenueList, "Tính doanh thu khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<CourseRevenueViewModel>>($"Lỗi khi tính doanh thu khóa học: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAgencyIdAsync(Guid AgencyId,DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<List<CourseRevenueViewModel>>();
            try
            {
                var agencyId = AgencyId;
                var courseRevenueList = new List<CourseRevenueViewModel>();
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(agencyId);
                var courseList = await _unitOfWork.CourseRepository.GetAllAsync();
                var contract = await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(agencyId);
                foreach (var course in courseList)
                {
                    var filteredRegisterCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCourseByCourseIdAsync(course.Id);
                    var TotalMoney = (double)0;
                    var studentCount = 0;
                    var validRegistrations = filteredRegisterCourses
                .Where(r => r.CreationDate.Date <= endDate.Date)
                .ToList();
                    foreach (var registration in validRegistrations)
                    {
                        studentCount++;
                        var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                        foreach (var payment in payments)
                        {
                            if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                            {
                                TotalMoney += payment.Amount ?? 0;
                            }
                        }
                    }
                    courseRevenueList.Add(new CourseRevenueViewModel
                    {
                        CourseId = course.Id,
                        StudentCount = studentCount,
                        CourseCode = course.Code,
                        CourseName = course.Name,
                        TotalRevenue = TotalMoney,
                        MonthlyFee= (double)(TotalMoney *contract.RevenueSharePercentage)
                    });
                }
                response = ResponseHandler.Success(courseRevenueList, "Tính doanh thu khóa học thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<List<CourseRevenueViewModel>>($"Lỗi khi tính doanh thu khóa học: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseAsync(DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {
                
                var userCurrentId = _claimsService.GetCurrentUserId;
                var userCurrent = await _userManager.FindByIdAsync(userCurrentId.ToString());
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(userCurrent.AgencyId.Value);
                var totalMoney = (double)0;
                if (userCurrent == null || !userCurrent.AgencyId.HasValue)
                {
                    return ResponseHandler.Success<double>(0,"User hoặc Agency không khả dụng!");
                }
                foreach (var registration in registerCourses)
                {
                    
                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;  
                        }
                    }
                }
                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetTotalRevenueFromRegisterCourseByAgencyIdAsync(Guid AgencyId,DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {

                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(AgencyId);
                var totalMoney = (double)0;
                foreach (var registration in registerCourses)
                {

                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;
                        }
                    }
                }
                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }
        public async Task<ApiResponse<double>> GetAmountAgencyAmountPayAsync(Guid AgencyId, DateTime startDate, DateTime endDate)
        {
            var response = new ApiResponse<double>();

            try
            {
                var contract =await _unitOfWork.ContractRepository.GetMostRecentContractByAgencyIdAsync(AgencyId);
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(AgencyId);
                var totalMoney = (double)0;
                foreach (var registration in registerCourses)
                {

                    var payments = await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                    foreach (var payment in payments)
                    {
                        if (payment.CreationDate.Date >= startDate.Date && payment.CreationDate.Date <= endDate.Date)
                        {
                            totalMoney += payment.Amount ?? 0;
                        }
                    }
                }
                totalMoney = totalMoney * (contract.RevenueSharePercentage.Value/100);

                response = ResponseHandler.Success(totalMoney, "Tính tổng doanh thu từ Payment thành công!");
            }
            catch (Exception ex)
            {
                response = ResponseHandler.Failure<double>($"Lỗi khi tính tổng doanh thu: {ex.Message}");
            }

            return response;
        }

    }
}

using AutoMapper;
using FluentValidation;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Handler;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.ViewModels.AgencyDashboardViewModels;
using FranchiseProject.Application.ViewModels.ClassViewModel;
using FranchiseProject.Application.ViewModels.ClassViewModels;
using FranchiseProject.Domain.Entity;
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
        public async Task<ApiResponse<List<CourseRevenueViewModel>>> GetCourseRevenueAsync(Guid agencyId)
        {
            var response = new ApiResponse<List<CourseRevenueViewModel>>();
            try
            {

                var courseRevenueList = new List<CourseRevenueViewModel>();
                var registerCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCoursesByAgencyIdAsync(agencyId);
                var courseList = await _unitOfWork.CourseRepository.GetAllAsync();
                foreach (var course in courseList)
                {
                    var filteredRegisterCourses = await _unitOfWork.AgencyDashboardRepository.GetRegisterCourseByCourseIdAsync(course.Id);
                    var TotalMoney = 0;
                    var studentCount = 0;
                    foreach (var registration in filteredRegisterCourses)
                    {
                        studentCount++;
                          var payments=await _unitOfWork.AgencyDashboardRepository.GetPaymentsByRegisterCourseIdAsync(registration.Id);
                       foreach (var payment in payments)
                        {
                            TotalMoney=TotalMoney+payment.Amount.Value;
                         }
                    }
                    courseRevenueList.Add(new CourseRevenueViewModel
                    {
                        CourseId= course.Id,
                        StudentCount= studentCount,
                        TotalRevenue=TotalMoney,
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

    }
}

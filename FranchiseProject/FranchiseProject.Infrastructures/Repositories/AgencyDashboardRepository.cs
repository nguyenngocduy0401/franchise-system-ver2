using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class AgencyDashboardRepository:IAgencyDashboardRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        public AgencyDashboardRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService,
             UserManager<User> userManager,
             RoleManager<Role> roleManager
        )
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<List<RegisterCourse>> GetRegisterCoursesByAgencyIdAsync(Guid agencyId)
        {
            try
            {
                var userIds = await _dbContext.Users
                                              .Where(u => u.AgencyId == agencyId)
                                              .Select(u => u.Id)
                                              .ToListAsync();

                if (!userIds.Any())
                {
                    return new List<RegisterCourse>(); 
                }
                var registerCourses = await _dbContext.RegisterCourses
                                                       .Where(rc => userIds.Contains(rc.UserId)&& 
                                                       (
                                                       rc.StudentCourseStatus==StudentCourseStatusEnum.Waitlisted||
                                                       rc.StudentCourseStatus==StudentCourseStatusEnum.Enrolled
                                                    ))
                                                       .ToListAsync();

                return registerCourses;
            }
            catch (Exception ex)
            {                throw new Exception($"Error retrieving register courses for agency {agencyId}: {ex.Message}");
            }
        }
        public async Task<List<Payment>> GetPaymentsByRegisterCourseIdAsync(Guid registerCourseId)
        {
            try
            {
                var payments = await _dbContext.Payments
                                               .Where(p => p.RegisterCourseId == registerCourseId)
                                               .ToListAsync();

                return payments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving payments for register course {registerCourseId}: {ex.Message}");
            }
        }
       
        public async Task<List<RegisterCourse >> GetRegisterCourseByCourseIdAsync (Guid courseId, Guid agencyId)
        {
            var registerCourse= await _dbContext.RegisterCourses.Where (rc=>rc.CourseId == courseId && rc.User.AgencyId == agencyId).ToListAsync();
            return registerCourse;
        }
        public async Task<List<RegisterCourse>> GetRegisterCoursesByConditionsAsync(DateTime endDate, Guid agencyId, Guid courseId)
        {
            return await (from rc in _dbContext.RegisterCourses
                          join u in _dbContext.Users on rc.UserId equals u.Id
                          where rc.CreationDate <= endDate
                                && u.AgencyId == agencyId
                                && rc.CourseId == courseId
                          select rc)
                         .ToListAsync();
        }
        public async Task<List<Payment>> GetPaymentsByConditionsAsync(DateTime startDate, DateTime endDate, Guid agencyId, Guid courseId)
        {
            return await (from p in _dbContext.Payments
                          join rc in _dbContext.RegisterCourses on p.RegisterCourseId equals rc.Id
                          where p.ToDate >= DateOnly.FromDateTime(startDate.Date)
                                && p.ToDate <= DateOnly.FromDateTime(endDate.Date)
                                && p.AgencyId == agencyId
                                && rc.CourseId == courseId
                          select p)
                         .ToListAsync();
        }

    }
}


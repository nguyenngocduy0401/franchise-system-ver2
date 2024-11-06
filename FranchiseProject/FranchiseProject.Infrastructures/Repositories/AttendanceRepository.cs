using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        public AttendanceRepository(
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
        public async Task AddAsync(Attendance attendance)
        {
            await _dbContext.Set<Attendance>().AddAsync(attendance);
        }
        public void DeleteRange(List<Attendance> attendance)
        {
            _dbContext.Attendances.RemoveRange(attendance);
        }
        public async Task<List<Attendance>> GetAllAsync(Expression<Func<Attendance, bool>> predicate)
        {
            return await _dbContext.Set<Attendance>().Where(predicate).ToListAsync();
        }
        public async Task UpdateAsync(Attendance attendance)
        {

            var existingRegisterCourse = await _dbContext.Set<Attendance>()
        .FirstOrDefaultAsync(rc => rc.UserId == attendance.UserId && rc.ClassScheduleId == attendance.ClassScheduleId);

            if (existingRegisterCourse != null)
            {

              
                existingRegisterCourse.Status = attendance.Status;

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("RegisterCourse not found.");
            }
        }
        public async Task<Attendance> GetFirstOrDefaultAsync(Expression<Func<Attendance, bool>> predicate)
        {
            return await _dbContext.Set<Attendance>().FirstOrDefaultAsync(predicate);
        }
    }
}

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
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        public StudentClassRepository(
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
        public async Task<IEnumerable<StudentClass>> GetFilterAsync(Expression<Func<StudentClass, bool>> filter)
        {
            return await _dbContext.StudentClasses
                .Include(sc => sc.User)  
                .Include(sc => sc.Class)    
                .Where(filter)
                .ToListAsync();
        }
        public async Task<int> CountStudentsByClassIdAsync(Guid classId)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var studentRole = roles.FirstOrDefault(r => r.Name == "Student");
            if (studentRole == null) return 0; 
            var usersInRole = await _userManager.GetUsersInRoleAsync(studentRole.Name);
            var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
            var studentCount = await _dbContext.StudentClasses
                .Where(sc => sc.ClassId == classId && userIdsInRole.Contains(sc.UserId))
                .CountAsync();

            return studentCount;
        }
        public async Task<List<StudentClass>> GetAllAsync(Expression<Func<StudentClass, bool>> predicate)
        {
            return await _dbContext.StudentClasses.Where(predicate).ToListAsync();
        }
        public async Task<List<ClassSchedule>> GetClassSchedulesByUserIdAndTermIdAsync(string userId, Guid termId)
        {
            var studentClasses = await _dbContext.StudentClasses
                .Where(sc => sc.UserId == userId)
                .Select(sc => sc.ClassId)
                .ToListAsync();

            if (!studentClasses.Any())
            {
                return new List<ClassSchedule>(); 
            }
            var classSchedules = await _dbContext.ClassSchedules
                .Include(cs => cs.Class) 
                .Where(cs => studentClasses.Contains(cs.ClassId.Value) && cs.Class.TermId == termId)
                .ToListAsync();

            return classSchedules;
        }
        public async Task<int> CountClassSchedulesByUserIdAndTermIdAsync(string userId, Guid termId)
        {
            var classIds = await _dbContext.StudentClasses
                .Where(sc => sc.UserId == userId)
                .Select(sc => sc.ClassId)
                .ToListAsync();

            return await _dbContext.ClassSchedules
                .CountAsync(cs => classIds.Contains(cs.ClassId) && cs.Class.TermId == termId);
        }
    }
}

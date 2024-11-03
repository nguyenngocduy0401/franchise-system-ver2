using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
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
    public class ClassRoomRepository : IClassRoomRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        public ClassRoomRepository(
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
        public async Task<IEnumerable<ClassRoom>> FindAsync(Expression<Func<ClassRoom, bool>> expression, string includeProperties = "")
        {
            IQueryable<ClassRoom> query = _dbContext.ClassRooms.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<ClassRoom>> GetFilterAsync(Expression<Func<ClassRoom, bool>> filter)
        {
            return await _dbContext.ClassRooms
                .Include(sc => sc.User)  
                .Include(sc => sc.Class)    
                .Where(filter)
                .ToListAsync();
        }
     
        
        public async Task AddAsync(ClassRoom classRoom)
        {
            await _dbContext.Set<ClassRoom>().AddAsync(classRoom);
        }
        public async Task<List<User>> GetWaitlistedStudentsAsync(List<string> studentIds)
        {
            return await _dbContext.Users
                .Where(u => studentIds.Contains(u.Id) && u.StudentStatus == StudentStatusEnum.Waitlisted)
                .ToListAsync();
        }


        public async Task<List<string>> GetInvalidStudentsAsync(List<string> studentIds)
        {
            return await _dbContext.Users
                .Where(u => studentIds.Contains(u.Id) && u.StudentStatus != StudentStatusEnum.Waitlisted)
                .Select(u => u.FullName)
                .ToListAsync();
        }
        public async Task<List<ClassRoom>> GetAllAsync(Expression<Func<ClassRoom, bool>> predicate)
        {
              if (predicate == null)
            {
                return await _dbContext.Set<ClassRoom>().ToListAsync();
            }
            return await _dbContext.Set<ClassRoom>()
                .Where(predicate)
                .ToListAsync();
        }
        public async Task<ClassRoom> GetFirstOrDefaultAsync(Expression<Func<ClassRoom, bool>> predicate)
        {
            return await _dbContext.Set<ClassRoom>().FirstOrDefaultAsync(predicate);
        }

        public async Task DeleteAsync(ClassRoom entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<ClassRoom>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Dictionary<string, bool>> CheckWaitlistedStatusForStudentsAsync(List<string> studentIds, Guid courseId)
        {
            
            var waitlistedRecords = await _dbContext.Set<RegisterCourse>()
                .Where(rc => studentIds.Contains(rc.UserId) && rc.CourseId == courseId && rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted)
                .ToListAsync();
            var result = studentIds.ToDictionary(studentId => studentId, studentId =>
                waitlistedRecords.Any(rc => rc.UserId == studentId)
            );

            return result;
        }
        public async Task<Dictionary<string, bool>> CheckEnrollStatusForStudentsAsync(List<string> studentIds, Guid courseId)
        {

            var waitlistedRecords = await _dbContext.Set<RegisterCourse>()
                .Where(rc => studentIds.Contains(rc.UserId) && rc.CourseId == courseId && rc.StudentCourseStatus == StudentCourseStatusEnum.Enrolled)
                .ToListAsync();
            var result = studentIds.ToDictionary(studentId => studentId, studentId =>
                waitlistedRecords.Any(rc => rc.UserId == studentId)
            );

            return result;
        }
        public async Task<List<string>> GetUserIdsByClassIdAsync(Guid classId)
        {
            return await _dbContext.ClassRooms
                                 .Where(cr => cr.ClassId == classId)
                                 .Select(cr => cr.UserId)
                                 .ToListAsync();
        }

        public async Task<List<Guid>> GetClassIdsByCourseIdAsync(Guid courseId)
        {
            var classIds = await _dbContext.Classes
                .Where(c => c.CourseId == courseId)
                .Select(c => c.Id)
                .ToListAsync();

            return classIds;
        }
        public async Task<List<string>> GetInstructorUserIdsByClassIdAsync(Guid classId)
        {
            
            var userIds = await _dbContext.ClassRooms
                .Where(cr => cr.ClassId == classId)
                .Select(cr => cr.UserId)
                .ToListAsync();

            var instructorIds = new List<string>();

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user != null && await _userManager.IsInRoleAsync(user, AppRole.Instructor))
                {
                    instructorIds.Add(userId);
                }
            }

            return instructorIds;
        }
        public async Task<ClassRoom> GetClassRoomsByClassIdAndInstructorRoleAsync(Guid classId)
        {
            var instructorRoleId = await _dbContext.Roles
                .Where(r => r.Name == AppRole.Instructor)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            return await _dbContext.ClassRooms
                .Include(cr => cr.User)
                .Where(cr => cr.ClassId == classId &&
                             cr.User != null &&
                             cr.User.UserRoles.Any(ur => ur.RoleId == instructorRoleId))
                .FirstOrDefaultAsync();
        }

    }
}

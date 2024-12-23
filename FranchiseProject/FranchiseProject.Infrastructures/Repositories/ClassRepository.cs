using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ClassRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        
        public async Task<bool> CheckNameExistAsync(string name)
        {
            return await _dbContext.Classes.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }
        public async Task<Class> GetFirstOrDefaultAsync(Expression<Func<Class, bool>> filter)
        {
            return await _dbContext.Classes
                .FirstOrDefaultAsync(filter);
        }
        public async Task<int> CountAsync(Expression<Func<Class, bool>> filter = null)
        {
            IQueryable<Class> query = _dbContext.Classes;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }
        public async Task<List<User>> GetStudentsByClassIdAsync(Guid classId)
        {
            var studentRoleId = await _dbContext.Roles
                                                 .Where(r => r.Name == AppRole.Student)
                                                 .Select(r => r.Id)
                                                 .FirstOrDefaultAsync();

            return await _dbContext.Users
                                 .Where(u => u.Status == UserStatusEnum.active
                                              && u.UserRoles.Any(ur => ur.RoleId == studentRoleId)
                                              && u.ClassRooms.Any(cr => cr.ClassId == classId))
                                 .ToListAsync();
        }
        public async Task<List<Class>> GetClassesByUserIdAsync(string userId)
        {
            return await _dbContext.ClassRooms
                .Where(cr => cr.UserId == userId && cr.Class.Status == ClassStatusEnum.Active)
                .Include(cr => cr.Class)
                .Select(cr => cr.Class)
                .Where(c => c != null)
                .ToListAsync();
        }
        public async Task<List<Class>> GetAllAsync1(Expression<Func<Class, bool>> predicate)
        {
            return await _dbContext.Set<Class>().Where(predicate).ToListAsync();
        }
    }
}

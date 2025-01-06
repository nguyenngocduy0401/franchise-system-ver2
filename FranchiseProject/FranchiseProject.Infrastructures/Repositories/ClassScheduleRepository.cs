using FranchiseProject.Application;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class ClassScheduleRepository : GenericRepository<ClassSchedule>, IClassScheduleRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ClassScheduleRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        
        public async Task<ClassSchedule?> GetExistingScheduleAsync(DateTime date, string room, Guid slotId)
        {
            return  _dbContext.ClassSchedules
                .FirstOrDefault(cs => cs.Date == date &&
                                           
                                            cs.SlotId == slotId);
        }
        public async Task<List<ClassSchedule>> GetAllClassScheduleAsync(Expression<Func<ClassSchedule, bool>> predicate)
        {
            return await _dbContext.Set<ClassSchedule>().Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<ClassSchedule>> GetAllClassScheduleAsync(Expression<Func<ClassSchedule, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<ClassSchedule> query = _dbContext.ClassSchedules;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property.Trim());
                }
            }

            return await query.ToListAsync();
        }
        public async Task<ClassSchedule?> GetEarliestClassScheduleByClassIdAsync(Guid classId)
        {
            return await _dbContext.ClassSchedules
                .Where(cs => cs.ClassId == classId && cs.Date != null)
                .OrderBy(cs => cs.Date)
                .FirstOrDefaultAsync();
        }
        public async Task<ClassSchedule?> GetLatestClassScheduleByClassIdAsync(Guid classId)
        {
            return await _dbContext.ClassSchedules
                .Where(cs => cs.ClassId == classId && cs.Date != null)
                .OrderByDescending(cs => cs.Date)
                .FirstOrDefaultAsync();
        }
        public async Task<List<ClassSchedule>> GetClassSchedulesByClassIdsAsync(List<Guid> classIds, Expression<Func<ClassSchedule, bool>> predicate = null)
        {
            if (classIds == null || !classIds.Any())
                return new List<ClassSchedule>();

            var query = _dbContext.Set<ClassSchedule>().    Where(cs => cs.ClassId.HasValue && classIds.Contains(cs.ClassId.Value   ));
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync();
        }
        public async Task<ClassSchedule?> GetClassScheduleWithDetailsAsync(Guid id)
        {
            return await _dbContext.ClassSchedules
				.Include(cs => cs.Class)
				.Include(cs => cs.Class.Course)
				.Include(cs => cs.Slot)
                .Include(cs => cs.Attendances)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }
        public async Task<ClassSchedule> GetFirstOrDefaultAsync(Expression<Func<ClassSchedule, bool>> predicate)
        {
            return await _dbContext.Set<ClassSchedule>().FirstOrDefaultAsync(predicate);
        }
        public async Task<List<ClassSchedule>> GetAllAsync1(Expression<Func<ClassSchedule, bool>> predicate)
        {
            return await _dbContext.Set<ClassSchedule>().Where(predicate).ToListAsync();
        }
    }
}

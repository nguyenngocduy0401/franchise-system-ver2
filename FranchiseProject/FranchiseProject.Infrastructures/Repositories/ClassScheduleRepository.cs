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
        public async Task<List<ClassSchedule>> GetAllAsync1(Expression<Func<ClassSchedule, bool>> predicate)
        {
            return await _dbContext.Set<ClassSchedule>().Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<ClassSchedule>> GetAllAsync1(Expression<Func<ClassSchedule, bool>> filter = null, string includeProperties = "")
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
    }
}

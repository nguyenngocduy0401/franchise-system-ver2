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
    public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public AssignmentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<List<Assignment>> GetAllAsync1(Expression<Func<Assignment, bool>> predicate)
        {
            return await _dbContext.Set<Assignment>().Where(predicate).ToListAsync();
        }
        public async Task<Assignment> GetFirstOrDefaultAsync(Expression<Func<Assignment, bool>> predicate)
        {
            return await _dbContext.Set<Assignment>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Assignment>> GetAsmsByClassId(Guid classId)
        {
            return await _dbSet
                .Where(e => e.ClassId == classId && e.IsDeleted != true)
                          .Include(e => e.AssignmentSubmits).ThenInclude(s => s.User)
                          .ToListAsync();
        }
    }
}

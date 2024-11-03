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
    public class AssignmentSubmitRepository : IAssignmentSubmitRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;

        private readonly RoleManager<Role> _roleManager;
        public AssignmentSubmitRepository(
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
        public async Task AddAsync(AssignmentSubmit assignment)
        {
            await _dbContext.Set<AssignmentSubmit>().AddAsync(assignment);
        }
        public async Task DeleteAsync(AssignmentSubmit entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<AssignmentSubmit>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<AssignmentSubmit> GetFirstOrDefaultAsync(Expression<Func<AssignmentSubmit, bool>> predicate)
        {
            return await _dbContext.Set<AssignmentSubmit>().FirstOrDefaultAsync(predicate);
        }
        public async Task<List<AssignmentSubmit>> GetFilterAsync(Expression<Func<AssignmentSubmit, bool>> filter)
        {
            return await _dbContext.AssignmentSubmits
                .Include(sc => sc.User)
                .Include(sc => sc.Assignment)
                .Where(filter)
                .ToListAsync();
        }
        public async Task<List<AssignmentSubmit>> GetAllAsync1(Expression<Func<AssignmentSubmit, bool>> predicate)
        {
            return await _dbContext.Set<AssignmentSubmit>().Where(predicate).ToListAsync();
        }
        public async Task<List<AssignmentSubmit>> GetAllSubmissionsByAssignmentIdAsync(Guid assignmentId)
        {
            return await _dbContext.AssignmentSubmits
                .Where(rc => rc.AssignmentId == assignmentId)
                .ToListAsync();
        }
    }
}

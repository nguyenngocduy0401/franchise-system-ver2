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
    public class AgencyRepository : GenericRepository<Agency>, IAgencyRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        private readonly UserManager<User> _userManager;
        //private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        public AgencyRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService,
             UserManager<User> userManager
        
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
            _userManager = userManager;
          //  _roleManager = roleManager;
        }
        public async Task<IEnumerable<Agency>> GetAgencyExpiredAsync() 
        {
            return await _dbContext.Contracts
                        .AsNoTracking()
                        .Where(e => e.EndTime.HasValue &&
                                    _timeService.GetCurrentTime() >= e.EndTime.Value.AddDays(-40) &&
                                    _timeService.GetCurrentTime() <= e.EndTime.Value)
                        .Select(e => e.Agency).Where(e => e.Status != AgencyStatusEnum.Inactive)
                        .ToListAsync();
        }
        public async Task<IEnumerable<Agency>> GetAgencyEduLicenseExpiredAsync()
        {
            return await _dbContext.Documents
                        .AsNoTracking()
                        .Where(e => e.ExpirationDate.HasValue &&
                                    DateOnly.FromDateTime(_timeService.GetCurrentTime()) >= e.ExpirationDate.Value.AddDays(-40) &&
                                    DateOnly.FromDateTime(_timeService.GetCurrentTime()) <= e.ExpirationDate.Value)
                        .Select(e => e.Agency).Where(e => e.Status != AgencyStatusEnum.Inactive)
                        .ToListAsync();
        }
        public async Task<string?> GetAgencyManagerUserIdByAgencyIdAsync(Guid agencyId)
        {
            var users = await _userManager.Users
                .Where(u => u.AgencyId == agencyId)
            .ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, AppRole.AgencyManager))
                {
                    return user.Id;
                }
            }

            return null;
        }
        public async Task<List<Agency>> GetAllAsync1(Expression<Func<Agency, bool>> predicate)
        {
            return await _dbContext.Set<Agency>().Where(predicate).ToListAsync();
        }
    }
}

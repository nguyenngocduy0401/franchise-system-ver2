using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class AgencyRepository : GenericRepository<Agency>, IAgencyRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public AgencyRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
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
    }
}

using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class AgencyVnPayInfoRepository : GenericRepository<AgencyVnPayInfo>, IAgencyVnPayInfoRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public AgencyVnPayInfoRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<AgencyVnPayInfo> GetByAgencyIdAsync(Guid agencyId)
        {
            return await _dbContext.AgencyVnPayInfos
                .FirstOrDefaultAsync(info => info.AgencyId == agencyId);
        }
    }
}

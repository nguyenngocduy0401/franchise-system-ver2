using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class TermRepository : GenericRepository<Term>, ITermRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public TermRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<Term?> GetOverlappingTermsAsync(DateTime startDate, DateTime endDate)
        {
            return  _dbContext.Set<Term>()
                .FirstOrDefault(t =>
                    (startDate <= t.EndDate && endDate >= t.StartDate));
        }

        public async Task<Term?> GetByNameAsync(string name)
        {
            return  _dbContext.Set<Term>()
                .FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

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
    public class ChapterMaterialRepository : GenericRepository<ChapterMaterial>, IChapterMaterialRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ChapterMaterialRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
       /* public async Task<bool> HasOverlappingTermsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Set<Term>()
                .AnyAsync(t =>
                    (startDate < t.EndDate && endDate > t.StartDate));
        }
        public async Task<Term?> GetByNameAsync(string name)
        {
            return await _dbContext.Set<Term>()
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }*/
    }
}

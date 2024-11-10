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
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ScoreRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task AddAsync(Score score)
        {
            await _dbContext.Scores.AddAsync(score);
        }
        public async Task<IEnumerable<Score>> FindAsync(Expression<Func<Score, bool>> expression, string includeProperties = "")
        {
            IQueryable<Score> query = _dbContext.Scores.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
    }
}
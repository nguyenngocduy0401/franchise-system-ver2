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
    public class QuizDetailRepository : IQuizDetailRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public QuizDetailRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) 
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public void HardRemoveRange(List<QuizDetail> entities)
        {
            _dbContext.QuizDetails.RemoveRange(entities);
        }
        public Task<List<QuizDetail>> GetByQuizId(Guid quizId)
        {
           return _dbContext.QuizDetails
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
        }
        public async Task AddRangeAsync(List<QuizDetail> entities)
        {
            await _dbContext.QuizDetails.AddRangeAsync(entities);
        }

    }
}

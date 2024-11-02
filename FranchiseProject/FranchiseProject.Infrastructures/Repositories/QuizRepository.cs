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
    public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public QuizRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<Quiz> GetQuizForStudentById(Guid id)
        {
            return await _dbSet
                .Where(e => e.Id == id && e.IsDeleted != true)
                          .Include(e => e.QuizDetails)
                          .ThenInclude(e => e.Question)
                          .ThenInclude(e => e.QuestionOptions)
                          .FirstOrDefaultAsync();
            ;
        }
    }
}
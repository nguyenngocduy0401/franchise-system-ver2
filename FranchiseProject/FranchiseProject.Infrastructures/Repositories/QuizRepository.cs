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
        public async Task<IEnumerable<Quiz>> GetQuizScoreStudentByClassIdAndStudentId(Guid classId, string studentId)
        {
            return await _dbSet
                .Where(e => e.ClassId == classId  && e.IsDeleted != true)
                          .Include(e => e.Scores.Where(e => e.UserId == studentId))
                          .ToListAsync();
        }
        public async Task<Quiz> GetQuizScoreStudentByQuizIdAndStudentId(Guid id, string studentId)
        {
            return await _dbSet
                .Where(e => e.Id == id && e.IsDeleted != true)
                          .Include(e => e.Scores.Where(e => e.UserId == studentId))
                          .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizByClassId(Guid classId)
        {
            return await _dbSet
                .Where(e => e.ClassId == classId && e.IsDeleted != true)
                          .Include(e => e.Scores).ThenInclude(s => s.User)
                          .ToListAsync();
        }
    }
}
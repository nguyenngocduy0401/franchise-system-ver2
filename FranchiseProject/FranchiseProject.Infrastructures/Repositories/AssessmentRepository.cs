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
    public class AssessmentRepository : GenericRepository<Assessment>, IAssessmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public AssessmentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<List<Assessment>> GetAssessmentsByCourseIdAsync(Guid courseId) 
        {
            return await _dbContext.Courses
                .Where(c => c.Id == courseId && c.IsDeleted != true)
                .SelectMany(c => c.Assessments)
                .ToListAsync();
        }
        public async Task<List<Assessment>> GetAssessmentsByClassIdAsync(Guid classId, string userId)
        {
            return await _dbContext.Classes
            .Where(c => c.Id == classId && c.IsDeleted != true)
            .SelectMany(c => c.Course.Assessments)
            .Where(a => !a.IsDeleted)
            .Include(a => a.Assignments.Where(assign => assign.ClassId == classId && assign.Type == AssigmentTypeEnum.Compulsory && assign.IsDeleted != true))
                .ThenInclude(assign => assign.AssignmentSubmits.Where(submit => submit.UserId == userId))
            .Include(a => a.Quizzes.Where(quiz => quiz.ClassId == classId && quiz.Type == QuizTypeEnum.Compulsory && quiz.IsDeleted != true))
                .ThenInclude(quiz => quiz.Scores.Where(score => score.UserId == userId))
            .ToListAsync();
        }
    }
}

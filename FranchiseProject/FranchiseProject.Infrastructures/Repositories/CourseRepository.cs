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
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public CourseRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<Course> GetCourseDetailAsync(Guid courseId)
        {
            return await _dbSet
                .Where(e => e.Id == courseId)
                .Include(e => e.CourseMaterials) 
                .Include(e => e.Sessions.OrderBy(s => s.Number)) 
                .Include(e => e.Syllabus) 
                .Include(e => e.Assessments.OrderBy(a => a.Number)) 
                .Include(e => e.Chapters
                    .OrderBy(ch => ch.Number)) 
                    .ThenInclude(c => c.ChapterMaterials
                        .OrderBy(cm => cm.Number)) 
                .Include(e => e.CourseCategory)
                .FirstOrDefaultAsync();
        }
        public async Task<Course> GetCourseDetailForDuplicateAsync(Guid courseId)
        {
            return await _dbSet
                .Where(e => e.Id == courseId)
                .Include(e => e.CourseMaterials.Where(cm => !cm.IsDeleted != true))
                .Include(e => e.Sessions.Where(cm => cm.IsDeleted != true))
                .Include(e => e.Syllabus)
                .Include(e => e.Assessments.Where(a => a.IsDeleted != true))
                .Include(e => e.Chapters
                    .Where(c => c.IsDeleted != true))
                    .ThenInclude(c => c.ChapterMaterials
                        .Where(cm => cm.IsDeleted != true))
                  .Include(e => e.Chapters
                    .Where(c => c.IsDeleted != true))
                    .ThenInclude(c => c.Questions.Where(q => q.IsDeleted != true))
                    .ThenInclude(q => q.QuestionOptions.Where(qo => qo.IsDeleted != true))
                .Include(e => e.CourseCategory)
                .FirstOrDefaultAsync();
        }
    }
}


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
        .Include(e => e.Sessions)
        .Include(e => e.Syllabus)
        .Include(e => e.Assessments)
        .Include(e => e.Chapters)
        .ThenInclude(c => c.ChapterMaterials)
        .Include(e => e.CourseCategory)
        .FirstOrDefaultAsync();
        }
    }
}


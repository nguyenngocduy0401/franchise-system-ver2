using DocumentFormat.OpenXml.Spreadsheet;
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
using VimeoDotNet.Models;

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
        public async Task<bool> CheckUserInCourseAsync(string userId, Guid courseId)
        {
            return await _dbContext.ClassRooms
                .Where(cr => cr.UserId == userId)
                .Select(cr => cr.Class)
                .AnyAsync(c => c.CourseId == courseId);
        }
        public async Task<Course> GetCourseStudentAsync(Guid courseId)
        {
            return await _dbSet
                .Where(e => e.Id == courseId)
                .Include(e => e.CourseMaterials)
                .Include(e => e.Syllabus)
                .Include(e => e.Assessments.OrderBy(a => a.Number))
                .Include(e => e.Chapters
                    .OrderBy(ch => ch.Number))
                    .ThenInclude(c => c.ChapterMaterials
                    .OrderBy(cm => cm.Number))
                    .ThenInclude(c => c.UserChapterMaterials.Where(cm => cm.UserId == _claimsService.GetCurrentUserId.ToString()))
                .Include(e => e.CourseCategory)
                .FirstOrDefaultAsync();
        }
        public async Task<Course> GetCourseDetailAsync(Guid courseId)
        {
            return await _dbSet
                .Where(e => e.Id == courseId)
                .Include(e => e.CourseMaterials)
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


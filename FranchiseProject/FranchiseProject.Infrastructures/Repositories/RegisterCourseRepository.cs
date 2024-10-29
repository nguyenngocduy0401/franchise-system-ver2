using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class RegisterCourseRepository : IRegisterCourseRepository
    {
        private readonly AppDbContext _dbContext;
      
        public RegisterCourseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
           
        }

        public  Task<List<string>> GetCourseNamesByUserIdAsync(string userId)
        {
            return  _dbContext.RegisterCourses
            .Where(rc => rc.UserId == userId && rc.StudentCourseStatus == StudentCourseStatusEnum.Waitlisted) 
            .Include(rc => rc.Course) 
            .Select(rc => rc.Course.Name) 
            .ToListAsync();
            }
        public async Task AddAsync(RegisterCourse registerCourse)
        {
            await _dbContext.Set<RegisterCourse>().AddAsync(registerCourse);
        }
        public async Task UpdateAsync(RegisterCourse registerCourse)
        {

            var existingRegisterCourse = await _dbContext.Set<RegisterCourse>()
        .FirstOrDefaultAsync(rc => rc.UserId == registerCourse.UserId && rc.CourseId == registerCourse.CourseId);

            if (existingRegisterCourse != null)
            {
       
                existingRegisterCourse.DateTime = registerCourse.DateTime;
                existingRegisterCourse.StudentCourseStatus = registerCourse.StudentCourseStatus;

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("RegisterCourse not found.");
            }
        }
        public async Task<RegisterCourse?> GetFirstOrDefaultAsync(Expression<Func<RegisterCourse, bool>> filter)
        {
            return await _dbContext.RegisterCourses.FirstOrDefaultAsync(filter);
        }
        public async Task<bool> Update1Async(RegisterCourse registerCourse)
        {
            if (registerCourse == null)
            {
                throw new ArgumentNullException(nameof(registerCourse));
            }
            _dbContext.RegisterCourses.Update(registerCourse);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
        public async Task<List<RegisterCourse>> GetRegisterCoursesByUserIdAndStatusNullAsync(string userId)
        {
            return await _dbContext.RegisterCourses
                .Where(rc => rc.UserId == userId && rc.StudentCourseStatus == StudentCourseStatusEnum.Pending)
                .ToListAsync();
        }
        public void Delete(RegisterCourse registerCourse)
        {
            _dbContext.RegisterCourses.Remove(registerCourse);
        }
        public async Task<IEnumerable<RegisterCourse>> GetAllAsync(Expression<Func<RegisterCourse, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<RegisterCourse> query = _dbContext.RegisterCourses;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property.Trim());
                }
            }

            return await query.ToListAsync();
        }
    }
}

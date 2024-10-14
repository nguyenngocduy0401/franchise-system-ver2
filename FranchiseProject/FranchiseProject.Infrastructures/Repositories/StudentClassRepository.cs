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
    public class StudentClassRepository : IStudentClassRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public StudentClassRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) 
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<IEnumerable<StudentClass>> GetFilterAsync(Expression<Func<StudentClass, bool>> filter)
        {
            return await _dbContext.StudentClasses
                .Include(sc => sc.User)  // Bao gồm thông tin sinh viên
                .Include(sc => sc.Class)    // Bao gồm thông tin lớp học
                .Where(filter)
                .ToListAsync();
        }
    }
}

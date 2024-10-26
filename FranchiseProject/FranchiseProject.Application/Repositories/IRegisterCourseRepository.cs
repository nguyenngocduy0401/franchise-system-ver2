using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IRegisterCourseRepository
    {
        Task<List<string>> GetCourseNamesByUserIdAsync(string userId);
        Task AddAsync(RegisterCourse registerCourse);
        Task UpdateAsync(RegisterCourse registerCourse);
        Task<RegisterCourse?> GetFirstOrDefaultAsync(Expression<Func<RegisterCourse, bool>> filter);
        Task<bool> Update1Async(RegisterCourse registerCourse);
        Task<List<RegisterCourse>> GetRegisterCoursesByUserIdAndStatusNullAsync(string userId);
        void Delete(RegisterCourse registerCourse);
        Task<IEnumerable<RegisterCourse>> GetAllAsync(Expression<Func<RegisterCourse, bool>> filter = null, string includeProperties = "");
    }
}

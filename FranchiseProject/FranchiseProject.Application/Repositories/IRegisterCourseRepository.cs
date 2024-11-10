using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IRegisterCourseRepository :IGenericRepository<RegisterCourse>
    {
        Task<List<string>> GetCourseNamesByUserIdAsync(string userId);
        Task<List<string>> GetCourseCodeByUserIdAsync(string userId);
       
        Task UpdateAsync(RegisterCourse registerCourse);
        Task<RegisterCourse?> GetFirstOrDefaultAsync(Expression<Func<RegisterCourse, bool>> filter);
        Task<bool> UpdateVersion2Async(RegisterCourse registerCourse);
        Task<List<RegisterCourse>> GetRegisterCoursesByUserIdAndStatusNullAsync(string userId);
        void Delete(RegisterCourse registerCourse);
        Task<IEnumerable<RegisterCourse>> GetAllAsync(Expression<Func<RegisterCourse, bool>> filter = null, string includeProperties = "");
        Task<RegisterCourse> FindRegisterCourseByUserId(string userId, Guid courseId);
    }
}

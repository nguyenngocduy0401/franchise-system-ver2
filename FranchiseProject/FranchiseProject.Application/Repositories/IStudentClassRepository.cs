using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IStudentClassRepository
    {
        Task<IEnumerable<StudentClass>> GetFilterAsync(Expression<Func<StudentClass, bool>> filter);
        Task<int> CountStudentsByClassIdAsync(Guid classId);
        Task<List<StudentClass>> GetAllAsync(Expression<Func<StudentClass, bool>> predicate);
        Task<List<ClassSchedule>> GetClassSchedulesByUserIdAndTermIdAsync(string userId, Guid termId);
        Task<int> CountClassSchedulesByUserIdAndTermIdAsync(string userId, Guid termId);
    }
}

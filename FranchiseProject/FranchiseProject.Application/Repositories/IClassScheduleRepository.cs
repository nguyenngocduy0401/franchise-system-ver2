using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IClassScheduleRepository : IGenericRepository<ClassSchedule>
    {
        Task<ClassSchedule?> GetExistingScheduleAsync(DateTime date, string room, Guid slotId);
        Task<List<ClassSchedule>> GetAllAsync1(Expression<Func<ClassSchedule, bool>> predicate);

        Task<IEnumerable<ClassSchedule>> GetAllAsync1(Expression<Func<ClassSchedule, bool>> filter = null, string includeProperties = "");
        Task<ClassSchedule?> GetEarliestClassScheduleByClassIdAsync(Guid classId);
    }
}

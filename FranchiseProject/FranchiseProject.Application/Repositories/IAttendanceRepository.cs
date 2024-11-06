using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAttendanceRepository
    {
        Task AddAsync(Attendance attendance);
       void DeleteRange(List<Attendance> attendance);
        Task<List<Attendance>> GetAllAsync(Expression<Func<Attendance, bool>> predicate);
        Task UpdateAsync(Attendance attendance);
        Task<Attendance> GetFirstOrDefaultAsync(Expression<Func<Attendance, bool>> predicate);
    }
}

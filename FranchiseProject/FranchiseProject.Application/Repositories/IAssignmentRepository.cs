using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAssignmentRepository : IGenericRepository<Assignment>
    {
        Task<List<Assignment>> GetAllAsync1(Expression<Func<Assignment, bool>> predicate);
            Task<Assignment> GetFirstOrDefaultAsync(Expression<Func<Assignment, bool>> predicate);
        Task<IEnumerable<Assignment>> GetAsmsByClassId(Guid classId);
        Task<List<AssignmentSubmit>> GetAllSubmitAsync(Guid assignmentId);


    }
}

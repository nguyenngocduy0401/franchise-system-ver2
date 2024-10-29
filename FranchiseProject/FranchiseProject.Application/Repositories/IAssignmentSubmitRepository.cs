using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAssignmentSubmitRepository
    {
        Task AddAsync(AssignmentSubmit assignment);
        Task DeleteAsync(AssignmentSubmit entity);
        Task<AssignmentSubmit> GetFirstOrDefaultAsync(Expression<Func<AssignmentSubmit, bool>> predicate);
    }
}

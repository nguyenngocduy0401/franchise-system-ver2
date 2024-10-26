using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface ISlotRepository : IGenericRepository<Slot>
    {
        Task<Slot?> GetFirstOrDefaultAsync(Expression<Func<Slot, bool>> filter);
    }
}

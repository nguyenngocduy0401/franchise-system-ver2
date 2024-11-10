using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IScoreRepository
    {
        Task AddAsync(Score score);
        Task<IEnumerable<Score>> FindAsync(Expression<Func<Score, bool>> expression, string includeProperties = "");
    }
}

using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface ITermRepository : IGenericRepository<Term>
    {
        Task<Term?> GetOverlappingTermsAsync(DateTime startDate, DateTime endDate);
        Task<Term?> GetByNameAsync(string name);
    }
}

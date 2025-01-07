using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAgencyRepository : IGenericRepository<Agency>
    { 
        Task<IEnumerable<Agency>> GetAgencyExpiredAsync();
        Task<IEnumerable<Agency>> GetAgencyEduLicenseExpiredAsync();
        Task<string?> GetAgencyManagerUserIdByAgencyIdAsync(Guid agencyId);
        Task<List<Agency>> GetAllAsync1(Expression<Func<Agency, bool>> predicate);
    }
}

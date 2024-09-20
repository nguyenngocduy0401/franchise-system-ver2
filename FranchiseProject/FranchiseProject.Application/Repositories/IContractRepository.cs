using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<bool> IsExpiringContract(Guid contractId);
        Task<Contract> GetActiveContractByAgencyIdAsync(Guid agencyId);
    }
}

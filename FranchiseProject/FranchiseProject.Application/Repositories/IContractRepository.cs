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
        Task<Contract> GetMostRecentContractByAgencyIdAsync(Guid agencyId);
        Task<Contract> GetAllContractsByAgencyIdAsync(Guid agencyId);
        Task<bool> IsDepositPaidCorrectlyAsync(Guid contractId);

    }
}

using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IEquipmentRepository:IGenericRepository<Equipment>
    {
        Task<List<Equipment>> GetEquipmentByContractIdAsync(Guid contractId);
        
        Task<List<Equipment>> GetAllEquipmentsByAgencyIdAsync(Guid agencyId);
        Task<double> GetTotalEquipmentAmountByContractIdAsync(Guid contractId);
    }
}

using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class EquipmentRepository :GenericRepository<Equipment>,IEquipmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public EquipmentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<List<Equipment>> GetEquipmentByContractIdAsync(Guid contractId)
        {
            return await _dbContext.Equipments
                .Where(e => e.ContractId == contractId)
                .ToListAsync();
        }
        public async Task<List<Equipment>> GetAllEquipmentsByAgencyIdAsync(Guid agencyId)
        {
            return await _dbContext.Equipments
                .Include(e => e.Contract)
                .Where(e => e.Contract != null && e.Contract.AgencyId == agencyId)
                .ToListAsync();
        }
        public async Task<double> GetTotalEquipmentAmountByContractIdAsync(Guid contractId)
        {
            return (double)await _dbContext.Equipments
                .Where(e => e.ContractId == contractId && !e.IsDeleted)
                .SumAsync(e => e.Price );
        }

    }
}

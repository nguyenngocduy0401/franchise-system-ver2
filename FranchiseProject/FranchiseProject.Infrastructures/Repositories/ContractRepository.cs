﻿using FranchiseProject.Application.Interfaces;
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
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ContractRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<bool> IsExpiringContract(Guid contractId)
        {
            var contract = await _dbSet.FindAsync(contractId);
            return contract.EndTime > _timeService.GetCurrentTime();
        }
        public async Task<Contract> GetActiveContractByAgencyIdAsync(Guid agencyId)
        {
            return await _dbContext.Contracts
                .FirstOrDefaultAsync(c => c.AgencyId == agencyId && c.StartTime <= DateTime.Now && c.EndTime >= DateTime.Now);
        }
        public async Task<Contract> GetMostRecentContractByAgencyIdAsync(Guid agencyId)
        {
            return await _dbContext.Contracts
                .Where(c => c.AgencyId == agencyId).Include(c => c.Package)
                .OrderByDescending(c => c.CreationDate)
                .FirstOrDefaultAsync();
        }


        public async Task<Contract> GetAllContractsByAgencyIdAsync(Guid agencyId)
        {
            return  _dbContext.Contracts
                .Where(c => c.AgencyId == agencyId && c.Status == Domain.Enums.ContractStatusEnum.Active)
                .FirstOrDefault();
        }
        public async Task<bool> IsDepositPaidCorrectlyAsync(Guid contractId)
        {
            var contract = await _dbContext.Contracts
                .Where(c => c.Id == contractId)
                .Select(c => new { c.PaidAmount, c.Total, c.DepositPercentage })
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return false;
            }

            if (contract.PaidAmount == null || contract.Total == null || contract.DepositPercentage == null)
            {
                return false;
            }

            var expectedDeposit = contract.Total * contract.DepositPercentage / 100;
            return Math.Abs(contract.PaidAmount.Value - expectedDeposit.Value) < 0.01; 
        }
        public async Task<bool> IsCompletedPaidCorrectlyAsync(Guid contractId)
        {
            var contract = await _dbContext.Contracts
         .Where(c => c.Id == contractId)
         .Select(c => new { c.PaidAmount, c.Total })
         .FirstOrDefaultAsync();

            if (contract == null)
            {
                return false;
            }

            if (contract.PaidAmount == null || contract.Total == null)
            {
                return false;
            }

            return Math.Abs(contract.PaidAmount.Value - contract.Total.Value) < 0.01;
        }
    
    }
}


using FranchiseProject.Application;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.EquipmentViewModels;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public ReportRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<List<Equipment>> GetEquipmentsByReportIdAsync(Guid reportId)
        {
            var equipmentIds = await _dbContext.Reports
                .Where(r => r.Id == reportId)
                .SelectMany(r => r.Equipments.Select(e => e.Id))
                .ToListAsync();

            return await _dbContext.Equipments
                .Where(e => equipmentIds.Contains(e.Id))
                .ToListAsync();
        }
    }
}
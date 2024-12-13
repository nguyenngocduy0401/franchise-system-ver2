using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.DashBoard;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly AppDbContext _dbContext;
        public RevenueRepository(AppDbContext dbContext) { _dbContext = dbContext; }


        public async Task<List<RevenueData>> GetRevenueByMonthAsync(int year)
        {
            return  _dbContext.Payments
                .Where(p => p.CreationDate.Year == year)
                .GroupBy(p => new { p.CreationDate.Year, p.CreationDate.Month })
                .Select(g => new RevenueData
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Amount = (decimal)g.Sum(p => p.Amount ?? 0)
                })
                .ToList();
        }


        public async Task<List<PartnerRevenueShare>> GetMonthlyRevenueShareAsync()
        {
            var payments = await _dbContext.Payments
                .Where(p => p.Type == PaymentTypeEnum.MonthlyDue)
                .ToListAsync();

           // Console.WriteLine($"Payments count: {payments.Count}");

            var result = await _dbContext.Payments
                .Where(p => p.Type == PaymentTypeEnum.MonthlyDue)
                .Join(_dbContext.Contracts,
                      payment => payment.ContractId,
                      contract => contract.Id,
                      (payment, contract) => new { payment, contract.AgencyId })
                .GroupBy(pc => pc.AgencyId)
                .Select(g => new PartnerRevenueShare
                {
                    AgencyId = g.Key ?? Guid.Empty,
                    TotalAmount = (decimal)g.Sum(pc => pc.payment.Amount ?? 0)
                })
                .ToListAsync();

          //  Console.WriteLine($"Result count: {result.Count}");

            return result;
        }
    }
}

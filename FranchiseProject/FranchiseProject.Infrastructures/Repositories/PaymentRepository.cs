using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public PaymentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }

        public async Task<List<Payment>> GetAllAsync(Expression<Func<Payment, bool>> filter)
        {
            return await _dbContext.Set<Payment>()
                                 .Where(filter)
                                 .ToListAsync();
        }
        public async Task<Payment?> GetFirstOrDefaultAsync(Expression<Func<Payment, bool>> filter)
        {
            return await _dbContext.Payments.FirstOrDefaultAsync(filter);
        }
        public async Task<List<Payment>> GetPaymentsByAgencyIdAndDateRange(Guid agencyId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Payments
                .Where(p => p.RegisterCourse.User.AgencyId == agencyId &&
                            p.CreationDate >= startDate &&
                            p.CreationDate <= endDate)
                .ToListAsync();
        }
        public async Task<double> CalculateAgencyRevenue(Guid agencyId, DateTime startDate, DateTime endDate)
        {
            var payments = await _dbContext.Payments
                .Where(p => p.AgencyId == agencyId &&
                            p.CreationDate >= startDate &&
                            p.CreationDate <= endDate &&
                            p.Status == PaymentStatus.Completed)
                .ToListAsync();

            var revenue = payments
                .Where(p => p.Type != PaymentTypeEnum.Refund)
                .Sum(p => p.Amount ?? 0);

            var refunds = payments
                .Where(p => p.Type == PaymentTypeEnum.Refund)
                .Sum(p => p.Amount ?? 0);

            return revenue - refunds;
        }
    }
}

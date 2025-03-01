﻿using FranchiseProject.Application.Interfaces;
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
        //public async Task<double> CalculateAgencyRevenue(Guid agencyId, DateTime startDate, DateTime endDate)
        //{
        //    var payments = await _dbContext.Payments
        //        .Where(p => p.AgencyId == agencyId &&
        //                    p.ToDate.Value >= DateOnly.FromDateTime(startDate) &&
        //                    p.ToDate.Value <= DateOnly.FromDateTime(endDate) &&
        //                    p.Status == PaymentStatus.Completed)
        //        .ToListAsync();

        //    var revenue = payments
        //        .Where(p => p.Type != PaymentTypeEnum.Refund)
        //        .Sum(p => p.Amount ?? 0);

        //    var refunds = payments
        //        .Where(p => p.Type == PaymentTypeEnum.Refund)
        //        .Sum(p => p.Amount ?? 0);

        //    return revenue - refunds;
        //}
        public async Task<double> CalculateAgencyRevenue(Guid agencyId, DateTime startDate, DateTime endDate)
        {
            var totalAmountQuery = _dbContext.Payments
                .Where(p => p.AgencyId == agencyId &&
                         p.ToDate.HasValue &&
                    p.ToDate.Value >= DateOnly.FromDateTime(startDate.Date) &&
                    p.ToDate.Value <= DateOnly.FromDateTime(endDate.Date) &&
                            p.Type == PaymentTypeEnum.Course); // Lọc theo loại payment, giống như SQL

            // Tính tổng số tiền (Amount) cho các bản ghi thỏa mãn điều kiện, sau đó nhân với 28%
            var totalAmount = await totalAmountQuery
                .SumAsync(p => p.Amount ?? 0);

            var calculatedRevenue = totalAmount ; // Tính 28% tổng Amount

            return calculatedRevenue;
        }

        public async Task<double> GetTotalRevenueForAgencyInPeriod(Guid agencyId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Payments
                .Where(p => p.AgencyId == agencyId &&
                            p.ToDate.Value >= DateOnly.FromDateTime(startDate)  &&
                            p.ToDate.Value <= DateOnly.FromDateTime(endDate) &&
                            p.Status == PaymentStatus.Completed &&
                            p.Type != PaymentTypeEnum.Refund)
                .SumAsync(p => p.Amount ?? 0);
        }

        public async Task<double> GetTotalRefundsForAgencyInPeriod(Guid agencyId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Payments
                .Where(p => p.AgencyId == agencyId &&
                            p.ToDate.Value >= DateOnly.FromDateTime(startDate) &&
                            p.ToDate.Value <= DateOnly.FromDateTime(endDate) &&
                            p.Status == PaymentStatus.Completed &&
                            p.Type == PaymentTypeEnum.Refund)
                .SumAsync(p => p.Amount ?? 0);
        }
        public async Task<Payment?> GetPaymentByRegisterCourseIdAndUserId(Guid registerCourseId, string userId)
        {
            return await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.RegisterCourseId == registerCourseId && p.UserId == userId);
        }
    }
}

using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        Task<List<Payment>> GetAllAsync(Expression<Func<Payment, bool>> filter);
        Task<Payment?> GetFirstOrDefaultAsync(Expression<Func<Payment, bool>> filter);
        Task<List<Payment>> GetPaymentsByAgencyIdAndDateRange(Guid agencyId, DateTime startDate, DateTime endDate);
        Task<double> CalculateAgencyRevenue(Guid agencyId, DateTime startDate, DateTime endDate);
    }
}

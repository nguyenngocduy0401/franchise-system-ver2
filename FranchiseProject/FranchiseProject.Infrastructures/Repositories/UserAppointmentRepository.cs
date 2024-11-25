using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class UserAppointmentRepository : IUserAppointmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public UserAppointmentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<IEnumerable<UserAppointment>> GetAppointmentByUserId(Expression<Func<UserAppointment, bool>> expression, string includeProperties = "")
        {
            IQueryable<UserAppointment> query = _dbContext.UserAppointments.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<UserAppointment>> FindAsync(Expression<Func<UserAppointment, bool>> expression, string includeProperties = "")
        {
            IQueryable<UserAppointment> query = _dbContext.UserAppointments.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
        public void HardRemoveRange(List<UserAppointment> entities)
        {
            _dbContext.UserAppointments.RemoveRange(entities);
        }
        public async Task AddRangeAsync(List<UserAppointment> entities)
        {
            await _dbContext.UserAppointments.AddRangeAsync(entities);
        }
    }
}

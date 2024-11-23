using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Application.ViewModels.AppointmentViewModels;
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
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public AppointmentRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        ) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentByLoginAsync(string userId, Expression<Func<Appointment, bool>> expression) 
        {
            return await _dbContext.UserAppointments
                .Where(e => e.UserId == userId)
                .Select(up => up.Appointment)
                .Where(expression).ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentAgencyByLoginAsync(string userId, Expression<Func<Appointment, bool>> expression)
        {
            return await _dbContext.Users
                .Where(e => e.Id == userId)
                .SelectMany(up => up.Agency.Works)
                .SelectMany(w => w.Appointments).Where(expression).ToListAsync()
                ;
        }
        public async Task<Appointment> GetAppointmentAsyncById(Guid id) 
        {
            return await _dbContext.Appointments.Where(e => e.Id == id && e.IsDeleted != true)
                                          .Include(a => a.UserAppointments)
                                          .ThenInclude(u => u.User).FirstOrDefaultAsync();
        }
    }
}

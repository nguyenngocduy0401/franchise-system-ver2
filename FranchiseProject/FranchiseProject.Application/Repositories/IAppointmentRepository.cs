using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> GetAppointmentAsyncById(Guid id);
        Task<IEnumerable<Appointment>> GetAppointmentByLoginAsync(string userId, Expression<Func<Appointment, bool>> expression);
        Task<IEnumerable<Appointment>> GetAppointmentAgencyByLoginAsync(string userId, Expression<Func<Appointment, bool>> expression);
    }
}

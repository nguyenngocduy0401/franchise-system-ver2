using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IUserAppointmentRepository 
    {
        Task<IEnumerable<UserAppointment>> FindAsync(Expression<Func<UserAppointment, bool>> expression, string includeProperties = "");
        void HardRemoveRange(List<UserAppointment> entities);
        Task AddRangeAsync(List<UserAppointment> entities);
    }
}

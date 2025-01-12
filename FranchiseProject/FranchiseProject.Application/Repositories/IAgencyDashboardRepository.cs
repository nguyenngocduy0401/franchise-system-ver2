using FranchiseProject.Domain.Entity;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IAgencyDashboardRepository
    {
        Task<List<RegisterCourse>> GetRegisterCoursesByAgencyIdAsync(Guid agencyId);
        Task<List<Payment>> GetPaymentsByRegisterCourseIdAsync(Guid registerCourseId);
        Task<List<RegisterCourse>> GetRegisterCourseByCourseIdAsync(Guid courseId, Guid agencyId);
        Task<List<RegisterCourse>> GetRegisterCoursesByConditionsAsync(DateTime endDate, Guid agencyId, Guid courseId);
        Task<List<Payment>> GetPaymentsByConditionsAsync(DateTime startDate, DateTime endDate, Guid agencyId, Guid courseId);
    }
}

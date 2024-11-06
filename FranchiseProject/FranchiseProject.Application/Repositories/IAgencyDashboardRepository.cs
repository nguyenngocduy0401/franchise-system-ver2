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
        Task<List<RegisterCourse>> GetRegisterCourseByCourseIdAsync(Guid courseId);
    }
}

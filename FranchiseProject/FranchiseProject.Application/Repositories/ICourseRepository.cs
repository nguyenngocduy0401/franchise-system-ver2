using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course> GetCourseDetailAsync(Guid courseId);
        Task<Course> GetCourseDetailForDuplicateAsync(Guid courseId);
        Task<bool> CheckUserInCourseAsync(string userId, Guid courseId);
        Task<Course> GetCourseStudentAsync(Guid courseId);
    }
}

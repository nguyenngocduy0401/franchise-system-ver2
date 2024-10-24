using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IRegisterCourseRepository
    {
        Task<List<string>> GetCourseNamesByUserIdAsync(string userId);
        Task AddAsync(RegisterCourse registerCourse);
        Task UpdateAsync(RegisterCourse registerCourse);
    }
}

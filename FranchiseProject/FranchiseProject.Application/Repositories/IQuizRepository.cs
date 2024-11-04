using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IQuizRepository : IGenericRepository<Quiz>
    {
        Task<Quiz> GetQuizForStudentById(Guid id);
        Task<IEnumerable<Quiz>> GetQuizScoreStudentByClassIdAndStudentId(Guid classId, string studentId);
        Task<IEnumerable<Quiz>> GetQuizByClassId(Guid classId);
    }
}

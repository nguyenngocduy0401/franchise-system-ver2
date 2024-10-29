using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IClassRoomRepository
    {
        Task AddAsync(ClassRoom classRoom);
        Task<List<User>> GetWaitlistedStudentsAsync(List<string> studentIds);
        Task<List<string>> GetInvalidStudentsAsync(List<string> studentIds);
        Task<List<ClassRoom>> GetAllAsync(Expression<Func<ClassRoom, bool>> predicate);
        Task<ClassRoom> GetFirstOrDefaultAsync(Expression<Func<ClassRoom, bool>> predicate);
        Task DeleteAsync(ClassRoom entity);
        Task<Dictionary<string, bool>> CheckWaitlistedStatusForStudentsAsync(List<string> studentIds, Guid courseId);
        Task<Dictionary<string, bool>> CheckEnrollStatusForStudentsAsync(List<string> studentIds, Guid courseId);
        Task<List<string>> GetUserIdsByClassIdAsync(Guid classId);
    }
}

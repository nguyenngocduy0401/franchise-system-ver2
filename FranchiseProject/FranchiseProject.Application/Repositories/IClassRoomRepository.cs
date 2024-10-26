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
    }
}

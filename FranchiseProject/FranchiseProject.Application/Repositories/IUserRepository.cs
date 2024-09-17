using FranchiseProject.Application.Commons;
using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<List<string>> GetRolesByUserId(string userId);
        Task<Pagination<User>> GetFilterAsync(
           Expression<Func<User, bool>>? filter = null,
           Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
           string includeProperties = "",
           int? pageIndex = null,
           int? pageSize = null,
           string? role = null,
           string? foreignKey = null,
           object? foreignKeyId = null);
        Task<bool> CheckUserAttributeExisted(string attributeValue, string attributeType);
        Task<User> GetUserByUserNameAndPassword(string username, string password);
        Task AddAsync(User user);
        Task<string> GetUserByUserId(string userId);
        Task<string> GetCurrentUserRoleAsync(string userId);
    }
}

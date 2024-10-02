using FranchiseProject.Application.Commons;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.EntityFrameworkCore;
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
        Task<Pagination<User>> GetFilterAsync(
           Expression<Func<User, bool>>? filter = null,
           Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
           string includeProperties = "",
           int? pageIndex = null,
           int? pageSize = null,
           string? role = null,
           IsActiveEnum? isActive = null,
           string? foreignKey = null,
           object? foreignKeyId = null);
        Task<User> GetUserByUserName(string username);
        Task<User> GetUserByLogin(string username, string password);
        Task CreateUserAndAssignRoleAsync(User user, string role);
        Task<bool> CheckUserNameExistAsync(string username);


    }
}

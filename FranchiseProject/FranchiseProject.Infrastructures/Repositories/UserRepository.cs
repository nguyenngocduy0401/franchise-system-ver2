using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentTime _currentTime;
        private readonly RoleManager<Role> _roleManager;
        private readonly IClaimsService _claimsService;

        public UserRepository(AppDbContext dbContext, ICurrentTime currentTime,
            IClaimsService claimsService, UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            _dbContext = dbContext;
            _currentTime = currentTime;
            _claimsService = claimsService;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public virtual async Task<Pagination<User>> GetFilterAsync(
         Expression<Func<User, bool>>? filter = null,
         Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
         string includeProperties = "",
         int? pageIndex = null,
         int? pageSize = null,
         string? role = null,
         IsActiveEnum? isActive = null,
         string? foreignKey = null,
         object? foreignKeyId = null)
        {
            IQueryable<User> query = _dbContext.Users;
            if (isActive.HasValue)
            {
                switch (isActive)
                {
                    case IsActiveEnum.Active:
                        query = query.Where(u => 
                        (u.LockoutEnd <= DateTimeOffset.UtcNow || u.LockoutEnd == null) &&
                        (u.Contract == null || u.Contract.EndTime < _currentTime.GetCurrentTime()));
                        break;
                    case IsActiveEnum.Inactive:
                       
                        query = query.Where(u => (u.LockoutEnd > DateTimeOffset.UtcNow) || 
                        u.Contract != null && u.Contract.EndTime > _currentTime.GetCurrentTime());
                        break;
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIdsInRole = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIdsInRole.Contains(u.Id));
            }
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            var countTask = query.CountAsync();

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (!string.IsNullOrEmpty(foreignKey) && foreignKeyId != null)
            {
                if (foreignKeyId is Guid guidValue)
                {
                    query = query.Where(e => EF.Property<Guid>(e, foreignKey) == guidValue);
                }
                else if (foreignKeyId is string stringValue)
                {
                    query = query.Where(e => EF.Property<string>(e, foreignKey) == stringValue);
                }
                else
                {
                    throw new ArgumentException("Unsupported foreign key type");
                }
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var count = await countTask;
            var items = await query.ToListAsync();
            var result = new Pagination<User>
            {
                PageIndex = pageIndex ?? 0,
                PageSize = pageSize ?? 10,
                TotalItemsCount = count,
                Items = items
            };

            return result;
        }

        public async Task<User> GetUserByUserName(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)throw new Exception("Username or password is not correct!");
            
            return user;
        }
        public async Task<User> GetUserByLogin(string username, string password)
        {

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username
            && (u.LockoutEnd == null || u.LockoutEnd < _currentTime.GetCurrentTime()) &&
            (u.Contract == null || u.Contract.EndTime < _currentTime.GetCurrentTime()));
            if (user is null) throw new Exception("Username or password is not correct!");
            bool invalid = await _userManager.CheckPasswordAsync(user, password);
            if (invalid is false) throw new Exception("Username or password is not correct!");
            return user;
        }
        public async Task CreateUserAndAssignRoleAsync(User user, string role)
        {
            user.CreateAt = _currentTime.GetCurrentTime();
            var identityResult = await _userManager.CreateAsync(user, user.PasswordHash);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.ToString());
            identityResult = await _userManager.AddToRoleAsync(user, role);
            if(!identityResult.Succeeded) throw new Exception(identityResult.Errors.ToString());
        }

        public async Task<bool> CheckUserNameExistAsync(string username)
        {

            return await _dbContext.Users.AnyAsync(u => u.UserName == username);
        }
    }
}

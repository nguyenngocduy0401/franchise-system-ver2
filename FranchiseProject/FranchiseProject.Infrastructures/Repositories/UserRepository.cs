using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            IQueryable<User> query = _dbContext.Users
                .Include(u => u.UserRoles) 
                .ThenInclude(ur => ur.Role);
            if (isActive.HasValue)
            {
                switch (isActive)
                {
                    case IsActiveEnum.Active:
                        query = query.Where(u => 
                        (u.LockoutEnd <= DateTimeOffset.UtcNow || u.LockoutEnd == null)
                        );
                        break;
                    case IsActiveEnum.Inactive:
                       
                        query = query.Where(u => (u.LockoutEnd > DateTimeOffset.UtcNow)
                        );
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
            else
            {
                query = query.OrderByDescending(e => EF.Property<DateTime>(e, "CreateAt"));
                query = query.OrderByDescending(e => e.CreateAt);
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
            var user = await _dbContext.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)throw new Exception("Username or password is not correct!");
            
            return user;
        }
        public async Task<User> GetUserByLogin(string username, string password)
        {

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username
            && (u.LockoutEnd == null || u.LockoutEnd < _currentTime.GetCurrentTime()));
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
        public async Task<User> GetByAgencyIdAsync(Guid agencyId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.AgencyId == agencyId);
        }
        public async Task<Guid?> GetAgencyIdByUserIdAsync(string userId)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.AgencyId;
        }
        public async Task<User> GetStudentByIdAsync(string id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(AppRole.Student))
            {
                return null; 
            }
            return user;
        }
        public async Task<List<User>> GetAllAsync(Expression<Func<User, bool>> filter)
        {
            return await _dbContext.Users.Where(filter).ToListAsync();
        }
        public async Task<List<User>> GetInstructorsByAgencyIdAsync(Guid agencyId)
        {
            var instructorRoleId = await _dbContext.Roles
                                                 .Where(r => r.Name == AppRole.Instructor )
                                                 .Select(r => r.Id)
                                                 .FirstOrDefaultAsync();

            return await _dbContext.Users
                                 .Where(u => u.AgencyId == agencyId&&u.Status==UserStatusEnum.active && u.UserRoles.Any(ur => ur.RoleId == instructorRoleId))
                                 .ToListAsync();
        }
        public async Task<List<string>> GetAgencyUsersAsync(Guid agencyId)
        {
            var agencyManagerRole = await _roleManager.FindByNameAsync(AppRole.AgencyManager);
            var agencyStaffRole = await _roleManager.FindByNameAsync(AppRole.AgencyStaff);

            if (agencyManagerRole == null || agencyStaffRole == null)
            {
                throw new Exception("Required roles not found.");
            }

            var users = await _userManager.Users
                .Where(u => u.AgencyId == agencyId)
                .ToListAsync();

            var result = new List<string>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains(AppRole.AgencyManager) || userRoles.Contains(AppRole.AgencyStaff))
                {
                    result.Add(user.Id);
                }
            }

            return result;
        }
    }
}

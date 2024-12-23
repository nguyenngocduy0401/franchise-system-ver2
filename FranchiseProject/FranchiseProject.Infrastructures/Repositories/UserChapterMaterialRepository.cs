using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class UserChapterMaterialRepository : IUserChapterMaterialRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;
        public UserChapterMaterialRepository(
            AppDbContext context,
            ICurrentTime timeService,
            IClaimsService claimsService
        )
        {
            _dbContext = context;
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public async Task AddAsync(UserChapterMaterial userChapterMaterial)
        {
            userChapterMaterial.CompletedDate = _timeService.GetCurrentTime();
            await _dbContext.UserChapterMaterials.AddAsync(userChapterMaterial);
        }
        public async Task<IEnumerable<UserChapterMaterial>> FindAsync(Expression<Func<UserChapterMaterial, bool>> expression, string includeProperties = "")
        {
            IQueryable<UserChapterMaterial> query = _dbContext.UserChapterMaterials.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.ToListAsync();
        }
    }
}

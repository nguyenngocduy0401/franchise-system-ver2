using FranchiseProject.Application.Commons;
using FranchiseProject.Application.Interfaces;
using FranchiseProject.Application.Repositories;
using FranchiseProject.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Infrastructures.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected DbSet<TEntity> _dbSet;
        private readonly ICurrentTime _timeService;
        private readonly IClaimsService _claimsService;

        public GenericRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService)
        {
            _dbSet = context.Set<TEntity>();
            _timeService = timeService;
            _claimsService = claimsService;
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> expression, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.Where(expression);
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.ToList();
        }

        public virtual async Task<Pagination<TEntity>> GetFilterAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null,
            string? foreignKey = null,
            object? foreignKeyId = null)
        {
            IQueryable<TEntity> query = _dbSet;
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

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            var itemCount = await query.CountAsync();
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            var result = new Pagination<TEntity>()
            {
                PageIndex = pageIndex ?? 0,
                PageSize = pageSize ?? 10,
                TotalItemsCount = itemCount,
                Items = await query.ToListAsync(),
            };

            return result;
        }
        public Task<List<TEntity>> GetAllAsync() => _dbSet.ToListAsync();

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            entity.CreationDate = _timeService.GetCurrentTime();
            entity.CreatedBy = _claimsService.GetCurrentUserId;
            await _dbSet.AddAsync(entity);
        }

        public void SoftRemove(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletionDate = _timeService.GetCurrentTime();
            entity.DeleteBy = _claimsService.GetCurrentUserId;
            _dbSet.Update(entity);
        }
        public void RestoreSoftRemove(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.DeletionDate = _timeService.GetCurrentTime();
            entity.DeleteBy = _claimsService.GetCurrentUserId;
            _dbSet.Update(entity);
        }
        public void Update(TEntity entity)
        {
            entity.ModificationDate = _timeService.GetCurrentTime();
            entity.ModificationBy = _claimsService.GetCurrentUserId;
            _dbSet.Update(entity);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreationDate = _timeService.GetCurrentTime();
                entity.CreatedBy = _claimsService.GetCurrentUserId;
            }
            await _dbSet.AddRangeAsync(entities);
        }

        public void SoftRemoveRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.DeletionDate = _timeService.GetCurrentTime();
                entity.DeleteBy = _claimsService.GetCurrentUserId;
            }
            _dbSet.UpdateRange(entities);
        }
        public void UpdateRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.ModificationDate = _timeService.GetCurrentTime();
                entity.ModificationBy = _claimsService.GetCurrentUserId;
            }
            _dbSet.UpdateRange(entities);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }



    }
}

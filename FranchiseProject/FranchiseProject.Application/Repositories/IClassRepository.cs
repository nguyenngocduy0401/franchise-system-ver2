﻿using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<bool> CheckNameExistAsync(string name);
        Task<Class> GetFirstOrDefaultAsync(Expression<Func<Class, bool>> filter);
        Task<int> CountAsync(Expression<Func<Class, bool>> filter = null);
        Task<List<User>> GetStudentsByClassIdAsync(Guid classId);
        Task<List<Class>> GetClassesByUserIdAsync(string userId);
        Task<List<Class>> GetAllAsync1(Expression<Func<Class, bool>> predicate);
        IQueryable<Class> GetQueryable();
    }
}

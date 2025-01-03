﻿using FranchiseProject.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IUserChapterMaterialRepository
    {
        Task<double> CompletedPercentUserChapterMaterialAsync(Guid courseId, string userId);
        Task AddAsync(UserChapterMaterial userChapterMaterial);
        Task<IEnumerable<UserChapterMaterial>> FindAsync(Expression<Func<UserChapterMaterial, bool>> expression, string includeProperties = "");
        Task<bool> AnyAsync(Expression<Func<UserChapterMaterial, bool>> predicate);
    }
}

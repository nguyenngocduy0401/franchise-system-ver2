using DocumentFormat.OpenXml.Spreadsheet;
using FranchiseProject.Application.Commons;
using FranchiseProject.Application.ViewModels.WorkViewModels;
using FranchiseProject.Domain.Entity;
using FranchiseProject.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FranchiseProject.Application.Repositories
{
    public interface IWorkRepository : IGenericRepository<Work>
    { 
        Task<bool> CheckUserWorkExist(Guid workId, string userId);
        Task<Work> GetWorkDetailById(Guid id);
        IEnumerable<Work> GetAllPreWorkByAgencyId(Guid agencyId);
        Task<Pagination<Work>> FilterWorksByUserId(
            string userId,
            Expression<Func<Work, bool>>? filter = null,
            Func<IQueryable<Work>, IOrderedQueryable<Work>>? orderBy = null,
            string includeProperties = "",
            AppointmentTypeEnum? type = null,
            int? pageIndex = null,
            int? pageSize = null);
        Task<Work> GetPreviousWorkByAgencyId(Guid agencyId, Expression<Func<Work, bool>>? filter = null);
    }
}
